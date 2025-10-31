using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Visitka.Data;
using Visitka.Models;
using Visitka.Services;

namespace Visitka.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(2);

        public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AdminExistsAsync()
        {
            return await _context.AdminUsers.AnyAsync();
        }

        public async Task<bool> RegisterFirstAdminAsync(string username, string password)
        {
            if (await AdminExistsAsync())
                return false;

            var admin = new AdminUser
            {
                Username = username.Trim(),
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _context.AdminUsers.Add(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Username == username.Trim());

            if (admin == null || !VerifyPassword(password, admin.PasswordHash))
                return false;

            admin.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateToken();
            SetAuthCookie(token);

            return true;
        }

        public async Task<bool> ValidateTokenAsync()
        {
            var token = GetAuthCookie();
            if (string.IsNullOrEmpty(token))
                return false;

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Username == username.Trim());

            if (admin == null || !VerifyPassword(oldPassword, admin.PasswordHash))
                return false;

            admin.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();

            RemoveAuthCookie();

            return true;
        }

        public void Logout()
        {
            RemoveAuthCookie();
        }

        private void SetAuthCookie(string token)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.Add(_tokenLifetime),
                Path = "/admin"
            };

            httpContext.Response.Cookies.Append("AdminAuthToken", token, cookieOptions);
        }

        private string? GetAuthCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request.Cookies["AdminAuthToken"];
        }

        private void RemoveAuthCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            httpContext.Response.Cookies.Delete("AdminAuthToken", new CookieOptions
            {
                Path = "/admin"
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + GetSalt());
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }

        private string GenerateToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GetSalt()
        {
            return "your-secret-salt-key-here";
        }
    }
}
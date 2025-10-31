using Visitka.Models;

namespace Visitka.Services
{
    public interface IAuthService
    {
        Task<bool> AdminExistsAsync();
        Task<bool> RegisterFirstAdminAsync(string username, string password);
        Task<bool> LoginAsync(string username, string password);
        Task<bool> ValidateTokenAsync();
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        void Logout();
    }
}
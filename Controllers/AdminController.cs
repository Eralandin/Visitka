using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Visitka.Filters;
using Visitka.Models;
using Visitka.Services;

namespace Visitka.Controllers
{
    [ApiController]
    [Route("admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;

        public AdminController(IAdminService adminService, IAuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }

        [HttpGet("checkadmin")]
        [AllowAnonymous]
        public async Task<ActionResult> CheckAdminExists()
        {
            var adminExists = await _authService.AdminExistsAsync();
            return Ok(new { adminExists });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

            if (token == null)
                return Unauthorized(new { error = "Неверные учетные данные" });

            return Ok(new { token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterFirstAdmin([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest(new { error = "Имя пользователя и пароль обязательны" });

            if (loginDto.Password.Length < 6)
                return BadRequest(new { error = "Пароль должен содержать минимум 6 символов" });

            var result = await _authService.RegisterFirstAdminAsync(loginDto.Username, loginDto.Password);

            if (!result)
                return BadRequest(new { error = "Администратор уже существует" });

            return Ok(new { message = "Администратор успешно создан" });
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            _authService.Logout();
            return Ok(new { message = "Успешный выход" });
        }

        // Portfolio CRUD
        [HttpGet("portfolio")]
        public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolios()
        {
            var portfolios = await _adminService.GetPortfoliosAsync();
            return Ok(portfolios);
        }

        [HttpGet("portfolio/{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            var portfolio = await _adminService.GetPortfolioByIdAsync(id);
            if (portfolio == null)
                return NotFound();

            return Ok(portfolio);
        }

        [HttpPost("portfolio")]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromBody] Portfolio portfolio)
        {
            var createdPortfolio = await _adminService.CreatePortfolioAsync(portfolio);
            return CreatedAtAction(nameof(GetPortfolio), new { id = createdPortfolio.Id }, createdPortfolio);
        }

        [HttpPut("portfolio/{id}")]
        public async Task<ActionResult<Portfolio>> UpdatePortfolio(int id, [FromBody] Portfolio portfolio)
        {
            var updatedPortfolio = await _adminService.UpdatePortfolioAsync(id, portfolio);
            if (updatedPortfolio == null)
                return NotFound();

            return Ok(updatedPortfolio);
        }

        [HttpDelete("portfolio/{id}")]
        public async Task<ActionResult> DeletePortfolio(int id)
        {
            var result = await _adminService.DeletePortfolioAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Price CRUD
        [HttpGet("price")]
        public async Task<ActionResult<IEnumerable<Price>>> GetPrices()
        {
            var prices = await _adminService.GetPricesAsync();
            return Ok(prices);
        }

        [HttpGet("price/{id}")]
        public async Task<ActionResult<Price>> GetPrice(int id)
        {
            var price = await _adminService.GetPriceByIdAsync(id);
            if (price == null)
                return NotFound();

            return Ok(price);
        }

        [HttpPost("price")]
        public async Task<ActionResult<Price>> CreatePrice([FromBody] Price price)
        {
            var createdPrice = await _adminService.CreatePriceAsync(price);
            return CreatedAtAction(nameof(GetPrice), new { id = createdPrice.Id }, createdPrice);
        }

        [HttpPut("price/{id}")]
        public async Task<ActionResult<Price>> UpdatePrice(int id, [FromBody] Price price)
        {
            var updatedPrice = await _adminService.UpdatePriceAsync(id, price);
            if (updatedPrice == null)
                return NotFound();

            return Ok(updatedPrice);
        }

        [HttpDelete("price/{id}")]
        public async Task<ActionResult> DeletePrice(int id)
        {
            var result = await _adminService.DeletePriceAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Request Read-only
        [HttpGet("request")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            var requests = await _adminService.GetRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("request/{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _adminService.GetRequestByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var token = Request.Headers["X-Admin-Token"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var result = await _authService.ChangePasswordAsync(changePasswordDto.Username,
                changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (!result)
                return BadRequest(new { error = "Неверные данные" });

            return Ok(new { message = "Пароль успешно изменен" });
        }
    }

    public class ChangePasswordDto
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
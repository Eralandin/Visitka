using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Visitka.Services;

namespace Visitka.Filters
{
    public class AdminAuthFilter : IActionFilter
    {
        private readonly IAuthService _authService;

        public AdminAuthFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Проверяем, есть ли атрибут AllowAnonymous
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (!_authService.ValidateTokenAsync().Result)
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Требуется авторизация" });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Не требуется реализация
        }
    }
}
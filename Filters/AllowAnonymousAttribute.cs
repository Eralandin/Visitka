using Microsoft.AspNetCore.Mvc.Filters;

namespace   Visitka.Filters
{
    public class AllowAnonymousAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Ничего не делаем - разрешаем доступ
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Ничего не делаем
        }
    }
}
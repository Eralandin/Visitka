using Microsoft.AspNetCore.Mvc.Filters;

namespace   Visitka.Filters
{
    public class AllowAnonymousAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
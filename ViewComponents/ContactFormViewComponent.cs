using Microsoft.AspNetCore.Mvc;
using Visitka.Data;
using Visitka.Models;
using Microsoft.EntityFrameworkCore;

namespace Visitka.ViewComponents
{
    public class ContactFormViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ContactFormViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var prices = await _context.Prices.ToListAsync();
            return View(prices);
        }
    }
}

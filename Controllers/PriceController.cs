using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitka.Data;

namespace Visitka.Controllers
{
    [Route("Prices")]
    public class PriceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // localhost:8218/prices
        public async Task<IActionResult> Index()
        {
            var prices = await _context.Prices.ToListAsync();
            return View(prices);
        }
    }
}

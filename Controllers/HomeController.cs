using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Visitka.Data;
using Visitka.Models;

namespace Visitka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // localhost:8218/
        public IActionResult Index()
        {
            return View();
        }

        // localhost:8218/contacts
        [Route("Contacts")]
        public IActionResult Contacts()
        {
            return View();
        }
    }
}

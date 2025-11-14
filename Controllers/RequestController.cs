using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Visitka.Data;
using Visitka.Models;

namespace Visitka.Controllers
{
    [Route("Request")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // localhost:8218/request
        [HttpGet]
        public IActionResult Index()
        {
            if (TempData["InitialQuestion"] != null)
            {
                ViewData["InitialQuestion"] = TempData["InitialQuestion"].ToString();
            }
            else
            {
                ViewData["InitialQuestion"] = "";
            }
            return View();
        }

        // localhost:8218/request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Requests.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("Success");
            }
            return View(request);
        }

        // localhost:8218/request/success
        [Route("Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
}

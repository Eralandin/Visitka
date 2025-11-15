using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // localhost:8218/request?serviceId=1
        [HttpGet]
        public async Task<IActionResult> Index(int? serviceId)
        {
            // Получаем все услуги для выпадающего списка
            var prices = await _context.Prices
                .OrderBy(p => p.Id)
                .Select(p => new PriceViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    MinCost = p.MinCost,
                    HasImage = p.image != null && p.image.Length > 0,
                })
                .ToListAsync();

            if (serviceId.HasValue)
            {
                var selectedService = prices.FirstOrDefault(p => p.Id == serviceId.Value);
                if (selectedService != null)
                {
                    ViewData["SelectedServiceId"] = selectedService.Id;
                    ViewData["SelectedServiceName"] = selectedService.Name;
                }
            }

            if (TempData["InitialQuestion"] != null)
            {
                ViewData["InitialQuestion"] = TempData["InitialQuestion"].ToString();
            }
            else
            {
                ViewData["InitialQuestion"] = "";
            }

            return View(prices);
        }

        // localhost:8218/request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Request request)
        {
            // Отладочная информация
            if (!ModelState.IsValid)
            {
                // Логируем ошибки валидации
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Requests.Add(request);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    // Логируем ошибку базы данных
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    ModelState.AddModelError("", "Произошла ошибка при сохранении данных");
                }
            }
            // if (ModelState.IsValid)
            // {
            //     _context.Requests.Add(request);
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction("Success");
            // }

            // Если ошибки валидации - возвращаем обратно с данными
            var prices = await _context.Prices
                .OrderBy(p => p.Id)
                .Select(p => new PriceViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    MinCost = p.MinCost,
                    HasImage = p.image != null && p.image.Length > 0,
                })
                .ToListAsync();

            return View(prices);
        }

        // localhost:8218/request/success
        [Route("Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
}
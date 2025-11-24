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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Request request)
        {
            Console.WriteLine($"Received request: {request.ClientName}, {request.Email}, {request.PhoneNumber}");
            Console.WriteLine($"NameOfTask: {request.NameOfTask}");
            Console.WriteLine($"AgreeToPrivacy: {request.AgreeToPrivacy}");

            // Дополнительная проверка для селекта
            if (string.IsNullOrEmpty(request.NameOfTask) || request.NameOfTask == "Выберите спектр услуг")
            {
                ModelState.AddModelError("NameOfTask", "Выберите спектр услуг");
                Console.WriteLine("NameOfTask validation failed");
            }

            // Дополнительная проверка для чекбокса
            if (!request.AgreeToPrivacy)
            {
                ModelState.AddModelError("AgreeToPrivacy", "Необходимо согласие с политикой конфиденциальности");
                Console.WriteLine("AgreeToPrivacy validation failed");
            }

            Console.WriteLine($"Starting POST processing...");
    
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Model is valid, saving to database...");
                    
                    request.CreatedAt = DateTime.UtcNow;
                    _context.Requests.Add(request);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine("Data saved successfully, redirecting to Success...");
                    
                    // Проверьте, что редирект выполняется
                    var result = RedirectToAction("Success");
                    Console.WriteLine($"Redirect result type: {result.GetType()}");
                    
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    ModelState.AddModelError("", "Произошла ошибка при сохранении данных.");
                }
            }
            else
            {
                Console.WriteLine($"ModelState is invalid. Errors: {ModelState.ErrorCount}");
            }

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

            // Сохраняем введенные значения для повторного отображения
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key].AttemptedValue != null)
                {
                    ViewData[key] = ModelState[key].AttemptedValue;
                }
            }

            return View(prices);
        }

        [HttpGet]
        [Route("Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
}
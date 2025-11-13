using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using Visitka.Data;
using Visitka.Filters;
using Visitka.Models;
using Visitka.Services;

namespace Visitka.Controllers
{
    

    [ApiController]
    [Route("admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;



        private readonly ApplicationDbContext _db;


        public AdminController(IAdminService adminService, IAuthService authService, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _adminService = adminService;
            _authService = authService;
            _db = db;
            _env = env;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("Login");
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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // _authService.LoginAsync возвращает bool
            var success = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

            if (!success)
                return Unauthorized(new { error = "Неверные учетные данные" });

            // Сохраняем факт авторизации в сессии
            HttpContext.Session.SetString("IsAdminLoggedIn", "true");

            return Ok(new { success = true });
        }



        [HttpGet("/admin/panel")]
        public IActionResult Panel()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin"); // не авторизован → на вход

            return View("Panel");
        }




        [HttpPost("/admin/logout")]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();           // очищаем сессию
            return Redirect("/admin");             // редирект на страницу входа
        }




        [HttpPost("/admin/portfolio")]
        public async Task<IActionResult> PortfolioTable()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");
            // Загружаем все портфолио
            var portfolios = await _db.Portfolios.ToListAsync();

            // Загружаем все категории
            var categories = await _db.Categories.ToListAsync();

            // Загружаем связи PortfolioCategories
            var portfolioCategories = await _db.PortfolioCategories.ToListAsync();

            // Формируем ViewModel
            var vmList = portfolios.Select(p => new PortfolioViewMod
            {
                Id = p.Id,
                Name = p.Name,
                TaskDescription = p.TaskDescription,
                SolutionDescription = p.SolutionDescription,
                PreviewImage = p.PreviewImage,
                MainImage = p.MainImage,
                MobileImage = p.MobileImage,
                releasedate = p.releasedate,
                ppimage = p.ppimage,
                isnew = p.isnew,
                onmainpage = p.onmainpage,
                CategoryNames = portfolioCategories
                    .Where(pc => pc.PortfolioId == p.Id)
                    .Select(pc => categories.FirstOrDefault(c => c.Id == pc.CategoryId)?.Name ?? "")
                    .ToList()
            }).ToList();

            return PartialView("_PortfolioTable", vmList);
        }

        [HttpGet("/admin/portfolio/image/{id}")]
        public async Task<IActionResult> GetPortfolioImage(int id, string type)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var p = await _db.Portfolios.FindAsync(id);
            if (p == null) return NotFound();

            byte[]? data = type switch
            {
                "preview" => p.PreviewImage,
                "main" => p.MainImage,
                "mobile" => p.MobileImage,
                "pp" => p.ppimage,
                _ => null
            };
            if (data == null) return NotFound();
            return File(data, "image/png");
        }

        // GET: /admin/portfolio/edit/{id}
        [HttpGet("/admin/portfolio/edit/{id}")]
        public async Task<IActionResult> EditPortfolio(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var portfolio = await _db.Portfolios.FindAsync(id);
            if (portfolio == null) return NotFound();

            var allCategories = await _db.Categories.ToListAsync();
            ViewBag.AllCategories = allCategories;

            // Загружаем текущие категории портфолио
            var selectedCategoryIds = await _db.PortfolioCategories
                .Where(pc => pc.PortfolioId == id)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            ViewBag.SelectedCategoryIds = selectedCategoryIds;

            return View("EditPortfolio", portfolio);
        }

        [HttpPost("/admin/portfolio/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPortfolioPost(int id)
        {

            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var portfolio = await _db.Portfolios.FindAsync(id);
            if (portfolio == null) return NotFound();

            // Вручную читаем поля из формы (надёжнее при multipart + файлах)
            var form = Request.Form;

            // Текстовые поля
            portfolio.Name = form["Name"].FirstOrDefault() ?? portfolio.Name;
            portfolio.TaskDescription = form["TaskDescription"].FirstOrDefault() ?? portfolio.TaskDescription;
            portfolio.SolutionDescription = form["SolutionDescription"].FirstOrDefault() ?? portfolio.SolutionDescription;

            if (int.TryParse(form["releasedate"].FirstOrDefault(), out var rd))
                portfolio.releasedate = rd;

            // Чекбоксы: используем явную проверку значений "true"/"on"
            portfolio.onmainpage = form["onmainpage"].Last() switch
            {
                "true" => true,
                "on" => true,
                "1" => true,
                _ => false
            };

            portfolio.isnew = form["isnew"].LastOrDefault() switch
            {
                "true" => true,
                "on" => true,
                "1" => true,
                _ => false
            };

            var selectedCategoryIds = form["CategoryIds"].Select(int.Parse).ToList();

            // Удаляем старые связи
            var oldLinks = _db.PortfolioCategories.Where(pc => pc.PortfolioId == id);
            _db.PortfolioCategories.RemoveRange(oldLinks);

            // Добавляем новые
            foreach (var catId in selectedCategoryIds)
            {
                _db.PortfolioCategories.Add(new PortfolioCategories
                {
                    PortfolioId = id,
                    CategoryId = catId
                });
            }

            // Файлы: берем из Request.FormFiles
            var files = Request.Form.Files;
            var preview = files.FirstOrDefault(f => f.Name == "previewImage");
            var main = files.FirstOrDefault(f => f.Name == "mainImage");
            var mobile = files.FirstOrDefault(f => f.Name == "mobileImage");
            var pp = files.FirstOrDefault(f => f.Name == "ppImage");

            async Task<byte[]?> ReadFileAsync(IFormFile? f)
            {
                if (f == null || f.Length == 0) return null;
                using var ms = new MemoryStream();
                await f.CopyToAsync(ms);
                return ms.ToArray();
            }

            var previewBytes = await ReadFileAsync(preview);
            if (previewBytes != null) portfolio.PreviewImage = previewBytes;

            var mainBytes = await ReadFileAsync(main);
            if (mainBytes != null) portfolio.MainImage = mainBytes;

            var mobileBytes = await ReadFileAsync(mobile);
            if (mobileBytes != null) portfolio.MobileImage = mobileBytes;

            var ppBytes = await ReadFileAsync(pp);
            if (ppBytes != null) portfolio.ppimage = ppBytes;

            await _db.SaveChangesAsync();

            // PRG: редирект обратно на панель
            return Redirect("/admin/panel");
        }

        [HttpGet("portfolio/add")]
        public async Task<IActionResult> AddPortfolio()
        {

            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            // Загружаем все категории для мультиселекта / чекбоксов
            ViewBag.AllCategories = await _db.Categories.ToListAsync();
            ViewBag.SelectedCategoryIds = new List<int>(); // пока ничего не выбрано
            return View("EditPortfolio", new Portfolio()); // Используем ту же страницу, что и редактирование
        }

        [HttpPost("portfolio/add")]
        public async Task<IActionResult> AddPortfolioPost(IFormCollection form)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var portfolio = new Portfolio
            {
                Name = form["Name"],
                TaskDescription = form["TaskDescription"],
                SolutionDescription = form["SolutionDescription"],
                isnew = form["isnew"].Last() switch
                {
                    "true" => true,
                    "on" => true,
                    "1" => true,
                    _ => false
                },
                onmainpage = form["onmainpage"].Last() switch
                {
                    "true" => true,
                    "on" => true,
                    "1" => true,
                    _ => false
                },
                releasedate = int.TryParse(form["releasedate"].FirstOrDefault(), out var rd)?rd:0


            };

            // Обработка изображений (если есть)
            var files = new[] { "previewImage", "mainImage", "mobileImage", "ppimage" };
            foreach (var fileKey in files)
            {
                var file = form.Files[fileKey];
                if (file != null && file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    switch (fileKey)
                    {
                        case "previewImage": portfolio.PreviewImage = bytes; break;
                        case "mainImage": portfolio.MainImage = bytes; break;
                        case "mobileImage": portfolio.MobileImage = bytes; break;
                        case "ppimage": portfolio.ppimage = bytes; break;
                    }
                }
                else
                {
                    // Загружаем default.jpg
                    var wwwRoot = _env.WebRootPath;
                    var defaultBytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(wwwRoot, "default.jpg"));
                    switch (fileKey)
                    {
                        case "previewImage": portfolio.PreviewImage ??= defaultBytes; break;
                        case "mainImage": portfolio.MainImage ??= defaultBytes; break;
                        case "mobileImage": portfolio.MobileImage ??= defaultBytes; break;
                        case "ppimage": portfolio.ppimage ??= defaultBytes; break;
                    }
                }

            }

            _db.Portfolios.Add(portfolio);
            await _db.SaveChangesAsync();

            // Сохраняем категории
            var selectedCategoryIds = form["CategoryIds"].Select(int.Parse).ToList();
            foreach (var catId in selectedCategoryIds)
            {
                _db.PortfolioCategories.Add(new PortfolioCategories
                {
                    PortfolioId = portfolio.Id,
                    CategoryId = catId
                });
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Panel", "Admin");
        }



        [HttpPost("/admin/portfolio/delete/{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var portfolio = await _db.Portfolios.FindAsync(id);
            if (portfolio == null)
                return NotFound();

            _db.Portfolios.Remove(portfolio);
            await _db.SaveChangesAsync();
            return Ok();
        }



        [HttpPost("/admin/prices")]
        public async Task<IActionResult> GetPricesTable()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var prices = await _db.Prices
                .Select(p => new PriceViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    MinCost = p.MinCost,
                    HasImage = p.image != null && p.image.Length > 0
                })
                .ToListAsync();

            return PartialView("_PricesTable", prices);
        }

        [HttpGet("prices/edit/{id}")]
        public async Task<IActionResult> EditPrice(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var price = await _db.Prices.FindAsync(id);
            if (price == null)
                return NotFound();

            return View("EditPrice", price);
        }

        [HttpPost("prices/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPricePost(int id, IFormCollection form)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var price = await _db.Prices.FindAsync(id);
            if (price == null) return NotFound();

            price.Name = form["Name"];
            price.Description = form["Description"];
            if (int.TryParse(form["MinCost"], out var minCost)) price.MinCost = minCost;

            // Обработка изображения
            var file = form.Files["image"];
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                price.image = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            return Redirect("/admin/panel?tab=prices");
        }

        [HttpGet("price/image/{id}")]
        public async Task<IActionResult> GetPriceImage(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var price = await _db.Prices.FindAsync(id);
            if (price == null || price.image == null)
                return NotFound();

            return File(price.image, "image/png");
        }

        [HttpGet("prices/add")]
        public IActionResult AddPrice()
        {

            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            return View("EditPrice", new Price());
        }

        [HttpPost("prices/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPricePost(IFormCollection form)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var price = new Price
            {
                Name = form["Name"],
                Description = form["Description"],
            };
            if (int.TryParse(form["MinCost"], out var minCost)) price.MinCost = minCost;

            // Добавление изображения
            var file = form.Files["image"];
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                price.image = ms.ToArray();
            }
            else
            {
                // Загружаем default.jpg
                var wwwRoot = _env.WebRootPath;
                var defaultBytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(wwwRoot, "default.jpg"));
                price.image = defaultBytes;
            }

            _db.Prices.Add(price);
            await _db.SaveChangesAsync();

            return Redirect("/admin/panel?tab=prices");
        }

        [HttpPost("/admin/price/{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var price = await _db.Prices.FindAsync(id);
            if (price == null) return NotFound();

            _db.Prices.Remove(price);
            await _db.SaveChangesAsync();
            return Ok("/admin/panel?tab=prices");
        }



        [HttpPost("/admin/requests")]
        public async Task<IActionResult> RequestsTable()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var requests = await _db.Requests
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new Request
                {
                    Id = r.Id,
                    ClientName = r.ClientName,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    Telegram = r.Telegram,
                    NameOfTask = r.NameOfTask,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return PartialView("_RequestsTable", requests);
        }

        [HttpPost("requests/delete")]
        public async Task<IActionResult> DeleteRequest([FromForm] int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsAdminLoggedIn");
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                return Redirect("/admin");

            var request = await _db.Requests.FindAsync(id);
            if (request == null)
                return NotFound();

            _db.Requests.Remove(request);
            await _db.SaveChangesAsync();

            var requests = await _db.Requests
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return PartialView("_RequestsTable", requests);
        }



    }

    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class PortfolioViewMod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string SolutionDescription { get; set; } = string.Empty;

        public byte[]? PreviewImage { get; set; }
        public byte[]? MainImage { get; set; }
        public byte[]? MobileImage { get; set; }
        public byte[]? ppimage { get; set; }

        public int releasedate { get; set; }
        public bool isnew { get; set; }
        public bool onmainpage { get; set; }

        public List<string> CategoryNames { get; set; } = new();
    }


    public class ChangePasswordDto
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
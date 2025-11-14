using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitka.Data;
using Visitka.Models;

namespace Visitka.Controllers
{
    [Route("Portfolio")] // Все действия контроллера будут доступны по /Portfolio
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PortfolioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // localhost:8218/portfolio
        public async Task<IActionResult> Index()
        {
            var portfolios = await _context.Portfolios
                .OrderBy(p => p.Id)
                .Select(p => new PortfolioViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    TaskDescription = p.TaskDescription,
                    SolutionDescription = p.SolutionDescription,
                    HasPreviewImage = p.PreviewImage != null && p.PreviewImage.Length > 0,
                    HasMainImage = p.MainImage != null && p.MainImage.Length > 0,
                    HasMobileImage = p.MobileImage != null && p.MobileImage.Length > 0,
                    releasedate = p.releasedate,


                    Category = string.Join(", ",
                        _context.PortfolioCategories
                            .Where(pc => pc.PortfolioId == p.Id)
                            .Join(_context.Categories,
                                pc => pc.CategoryId,
                                c => c.Id,
                                (pc, c) => c.Name)
                    ),
                    isnew = p.isnew,
                    onmainpage = p.onmainpage,
                    HasPPImage = p.ppimage != null && p.ppimage.Length > 0
                })
                .ToListAsync();

            return View(portfolios);
        }


        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.Id == id)
                .Select(p => new PortfolioViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    TaskDescription = p.TaskDescription,
                    SolutionDescription = p.SolutionDescription,
                    HasPreviewImage = p.PreviewImage != null && p.PreviewImage.Length > 0,
                    HasMainImage = p.MainImage != null && p.MainImage.Length > 0,
                    HasMobileImage = p.MobileImage != null && p.MobileImage.Length > 0,
                    releasedate = p.releasedate,

                    Category = string.Join(", ",
                        _context.PortfolioCategories
                            .Where(pc => pc.PortfolioId == p.Id)
                            .Join(_context.Categories,
                                pc => pc.CategoryId,
                                c => c.Id,
                                (pc, c) => c.Name)
                    ),
                    isnew = p.isnew,
                    onmainpage = p.onmainpage,
                    HasPPImage = p.ppimage != null && p.ppimage.Length > 0
                })
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }


        // GET: /portfolio/image/ppreview/1
        [HttpGet("portfolio/image/ppreview/{id}")]
        public async Task<IActionResult> GetPPImage(int id)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.Id == id)
                .Select(p => new { p.ppimage, p.Name })
                .FirstOrDefaultAsync();

            return GetImageResult(portfolio?.ppimage, portfolio?.Name, "ppreview");
        }


        // GET: /portfolio/image/preview/1
        [HttpGet("portfolio/image/preview/{id}")]
        public async Task<IActionResult> GetPreviewImage(int id)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.Id == id)
                .Select(p => new { p.PreviewImage, p.Name })
                .FirstOrDefaultAsync();

            return GetImageResult(portfolio?.PreviewImage, portfolio?.Name, "preview");
        }

        // GET: /portfolio/image/main/1
        [HttpGet("portfolio/image/main/{id}")]
        public async Task<IActionResult> GetMainImage(int id)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.Id == id)
                .Select(p => new { p.MainImage, p.Name })
                .FirstOrDefaultAsync();

            return GetImageResult(portfolio?.MainImage, portfolio?.Name, "main");
        }

        // GET: /portfolio/image/mobile/1
        [HttpGet("portfolio/image/mobile/{id}")]
        public async Task<IActionResult> GetMobileImage(int id)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.Id == id)
                .Select(p => new { p.MobileImage, p.Name })
                .FirstOrDefaultAsync();

            return GetImageResult(portfolio?.MobileImage, portfolio?.Name, "mobile");
        }

        private IActionResult GetImageResult(byte[]? imageData, string? name, string imageType)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return NotFound($"Image {imageType} not found for portfolio {name}");
            }

            var contentType = GetImageContentType(imageData);
            var fileName = $"{name ?? "portfolio"}-{imageType}.{GetFileExtension(contentType)}";

            return File(imageData, contentType, fileName);
        }

        private string GetImageContentType(byte[] imageData)
        {
            if (imageData.Length > 1 && imageData[0] == 0xFF && imageData[1] == 0xD8)
                return "image/jpeg";

            if (imageData.Length > 0 && imageData[0] == 0x89 && imageData[1] == 0x50)
                return "image/png";

            if (imageData.Length > 1 && imageData[0] == 0x47 && imageData[1] == 0x49)
                return "image/gif";

            if (imageData.Length > 7 && imageData[0] == 0x42 && imageData[1] == 0x4D)
                return "image/bmp";

            return "image/jpeg";
        }

        private string GetFileExtension(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                _ => "jpg"
            };
        }

    }


}

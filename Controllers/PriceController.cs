using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitka.Data;
using Visitka.Models;

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

        [HttpGet("price/image/{id}")]
        public async Task<IActionResult> GetPreviewImage(int id)
        {
            var price = await _context.Prices
                .Where(p => p.Id == id)
                .Select(p => new { p.image, p.Name })
                .FirstOrDefaultAsync();

            return GetImageResult(price?.image, price?.Name, "preview");
        }

        private IActionResult GetImageResult(byte[]? imageData, string? name, string imageType)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return NotFound($"Image {imageType} not found for price {name}");
            }

            var contentType = GetImageContentType(imageData);
            var fileName = $"{name ?? "price"}-{imageType}.{GetFileExtension(contentType)}";

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

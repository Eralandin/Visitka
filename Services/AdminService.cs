using Microsoft.EntityFrameworkCore;
using Visitka.Data;
using Visitka.Models;
using Visitka.Services;

namespace Visitka.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync()
        {
            return await _context.Portfolios.ToListAsync();
        }

        public async Task<Portfolio?> GetPortfolioByIdAsync(int id)
        {
            return await _context.Portfolios.FindAsync(id);
        }

        public async Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio?> UpdatePortfolioAsync(int id, Portfolio portfolio)
        {
            var existingPortfolio = await _context.Portfolios.FindAsync(id);
            if (existingPortfolio == null)
                return null;

            existingPortfolio.Name = portfolio.Name;
            existingPortfolio.TaskDescription = portfolio.TaskDescription;
            existingPortfolio.SolutionDescription = portfolio.SolutionDescription;
            existingPortfolio.PreviewImage = portfolio.PreviewImage;
            existingPortfolio.MainImage = portfolio.MainImage;
            existingPortfolio.MobileImage = portfolio.MobileImage;

            await _context.SaveChangesAsync();
            return existingPortfolio;
        }

        public async Task<bool> DeletePortfolioAsync(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
                return false;

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Price>> GetPricesAsync()
        {
            return await _context.Prices.ToListAsync();
        }

        public async Task<Price?> GetPriceByIdAsync(int id)
        {
            return await _context.Prices.FindAsync(id);
        }

        public async Task<Price> CreatePriceAsync(Price price)
        {
            _context.Prices.Add(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task<Price?> UpdatePriceAsync(int id, Price price)
        {
            var existingPrice = await _context.Prices.FindAsync(id);
            if (existingPrice == null)
                return null;

            existingPrice.Name = price.Name;
            existingPrice.Description = price.Description;
            existingPrice.MinCost = price.MinCost;

            await _context.SaveChangesAsync();
            return existingPrice;
        }

        public async Task<bool> DeletePriceAsync(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null)
                return false;

            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Request>> GetRequestsAsync()
        {
            return await _context.Requests.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<Request?> GetRequestByIdAsync(int id)
        {
            return await _context.Requests.FindAsync(id);
        }
    }
}
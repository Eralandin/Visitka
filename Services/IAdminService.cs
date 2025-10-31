using Visitka.Models;

namespace Visitka.Services
{
    public interface IAdminService
    {
        // Portfolio CRUD
        Task<IEnumerable<Portfolio>> GetPortfoliosAsync();
        Task<Portfolio?> GetPortfolioByIdAsync(int id);
        Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio);
        Task<Portfolio?> UpdatePortfolioAsync(int id, Portfolio portfolio);
        Task<bool> DeletePortfolioAsync(int id);

        // Price CRUD
        Task<IEnumerable<Price>> GetPricesAsync();
        Task<Price?> GetPriceByIdAsync(int id);
        Task<Price> CreatePriceAsync(Price price);
        Task<Price?> UpdatePriceAsync(int id, Price price);
        Task<bool> DeletePriceAsync(int id);

        // Request Read-only
        Task<IEnumerable<Request>> GetRequestsAsync();
        Task<Request?> GetRequestByIdAsync(int id);
    }
}
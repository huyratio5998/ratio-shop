using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IStockService
    {
        IEnumerable<Stock> GetStocks();
        Stock? GetStock(int id);
        Task<Stock> CreateStock(Stock Stock);
        bool UpdateStock(Stock Stock);
        bool DeleteStock(int id);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IStockRepository
    {
        IQueryable<Stock> GetStocks();
        Stock? GetStock(int id);
        Task<Stock> CreateStock(Stock Stock);
        bool UpdateStock(Stock Stock);        
        bool DeleteStock(int id);
    }
}

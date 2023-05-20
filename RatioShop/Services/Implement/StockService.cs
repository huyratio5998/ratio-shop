using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _StockRepository;

        public StockService(IStockRepository StockRepository)
        {
            _StockRepository = StockRepository;
        }

        public Task<Stock> CreateStock(Stock Stock)
        {
            Stock.CreatedDate = DateTime.UtcNow;
            Stock.ModifiedDate = DateTime.UtcNow;
            return _StockRepository.CreateStock(Stock);
        }

        public bool DeleteStock(int id)
        {
            return _StockRepository.DeleteStock(id);
        }

        public IEnumerable<Stock> GetStocks()
        {
            return _StockRepository.GetStocks();
        }

        public Stock? GetStock(int id)
        {
            return _StockRepository.GetStock(id);
        }

        public bool UpdateStock(Stock Stock)
        {
            Stock.ModifiedDate = DateTime.UtcNow;
            return _StockRepository.UpdateStock(Stock);
        }
    }
}

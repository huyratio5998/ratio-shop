using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class StockRepository : BaseEntityRepository<Stock>, IStockRepository
    {
        public StockRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Stock> CreateStock(Stock Stock)
        {
            return await Create(Stock);
        }

        public bool DeleteStock(int id)
        {
            return Delete(id);
        }

        public IQueryable<Stock> GetStocks()
        {
            return GetAll();
        }

        public Stock? GetStock(int id)
        {
            return GetById(id);
        }

        public bool UpdateStock(Stock Stock)
        {
            return Update(Stock);
        }
    }
}

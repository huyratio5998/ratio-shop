using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductVariantStockRepository : IProductVariantStockRepository
    {
        protected ApplicationDbContext _context;
        public ProductVariantStockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            await _context.AddAsync(ProductVariantStock);
            _context.SaveChanges();
            return ProductVariantStock;
        }

        public bool DeleteProductVariantStock(int StockId, Guid ProductVariantId)
        {
            var entity = GetProductVariantStock(StockId, ProductVariantId);
            if (entity == null) return false;

            _context.Set<ProductVariantStock>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId)
        {
            if (StockId == 0 || ProductVariantId == null || ProductVariantId == Guid.Empty) return null;

            return _context.Set<ProductVariantStock>().AsQueryable().AsNoTracking().FirstOrDefault(x => x.StockId == StockId && x.ProductVariantId.ToString().ToLower().Equals(ProductVariantId.ToString().ToLower()));
        }

        public IQueryable<ProductVariantStock> GetProductVariantStocks()
        {
            return _context.Set<ProductVariantStock>().AsQueryable().AsNoTracking();
        }

        public IEnumerable<ProductVariantStock> GetProductVariantStocks(int pageIndex, int pageSize)
        {
            return _context.Set<ProductVariantStock>()
                .AsQueryable()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        public bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            try
            {
                _context.Set<ProductVariantStock>().Update(ProductVariantStock);
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

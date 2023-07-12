using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        protected ApplicationDbContext _context;
        public ProductCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductCategory> CreateProductCategory(ProductCategory ProductCategory)
        {
            await _context.AddAsync(ProductCategory);
            _context.SaveChanges();
            return ProductCategory;
        }

        public bool DeleteProductCategory(int CategoryId, Guid ProductId)
        {
            var entity = GetProductCategory(CategoryId, ProductId);
            if (entity == null) return false;

            _context.Set<ProductCategory>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public ProductCategory? GetProductCategory(int CategoryId, Guid ProductId)
        {
            if (CategoryId == 0 || ProductId == null || ProductId == Guid.Empty) return null;

            return _context.Set<ProductCategory>().AsNoTracking().FirstOrDefault(x => x.CategoryId == CategoryId && x.ProductId.ToString().ToLower().Equals(ProductId.ToString().ToLower()));
        }

        public IQueryable<ProductCategory> GetProductCategorys()
        {
            return _context.Set<ProductCategory>().AsNoTracking();
        }

        public IQueryable<ProductCategory> GetProductCategorys(int pageIndex, int pageSize)
        {
            return _context.Set<ProductCategory>()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        public bool UpdateProductCategory(ProductCategory ProductCategory)
        {
            try
            {
                _context.Set<ProductCategory>().Update(ProductCategory);
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

using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductVariantPackageRepository : IProductVariantPackageRepository
    {
        protected ApplicationDbContext _context;
        public ProductVariantPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariantPackage> CreateProductVariantPackage(ProductVariantPackage ProductVariantPackage)
        {
            await _context.AddAsync(ProductVariantPackage);
            _context.SaveChanges();
            return ProductVariantPackage;
        }

        public bool DeleteProductVariantPackage(Guid PackageId, Guid ProductVariantId)
        {
            var entity = GetProductVariantPackage(PackageId, ProductVariantId);
            if (entity == null) return false;

            _context.Set<ProductVariantPackage>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public ProductVariantPackage? GetProductVariantPackage(Guid PackageId, Guid ProductVariantId)
        {
            if (PackageId == Guid.Empty || ProductVariantId == Guid.Empty) return null;

            return _context.Set<ProductVariantPackage>().AsNoTracking().FirstOrDefault(x => x.PackageId == PackageId && x.ProductVariantId.ToString().ToLower().Equals(ProductVariantId.ToString().ToLower()));
        }

        public IQueryable<ProductVariantPackage> GetProductVariantPackages()
        {
            return _context.Set<ProductVariantPackage>().AsNoTracking();
        }

        public IQueryable<ProductVariantPackage> GetProductVariantPackages(int pageIndex, int pageSize)
        {
            return _context.Set<ProductVariantPackage>()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        public bool UpdateProductVariantPackage(ProductVariantPackage ProductVariantPackage)
        {
            try
            {
                _context.Set<ProductVariantPackage>().Update(ProductVariantPackage);
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

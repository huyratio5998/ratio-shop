using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductVariantPackageRepository
    {
        IQueryable<ProductVariantPackage> GetProductVariantPackages();
        IQueryable<ProductVariantPackage> GetProductVariantPackages(int pageIndex, int pageSize);
        ProductVariantPackage? GetProductVariantPackage(Guid CategoryId, Guid ProductId);
        Task<ProductVariantPackage> CreateProductVariantPackage(ProductVariantPackage ProductVariantPackage);
        bool UpdateProductVariantPackage(ProductVariantPackage ProductVariantPackage);        
        bool DeleteProductVariantPackage(Guid CategoryId, Guid ProductId);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantPackageService
    {
        IEnumerable<ProductVariantPackage> GetProductVariantPackages();        
        ProductVariantPackage? GetProductVariantPackage(Guid PackageId, Guid variantId);
        Task<ProductVariantPackage> CreateProductVariantPackage(ProductVariantPackage ProductVariantPackage);
        bool UpdateProductVariantPackage(ProductVariantPackage ProductVariantPackage);
        bool DeleteProductVariantPackage(Guid PackageId, Guid variantId);
    }
}

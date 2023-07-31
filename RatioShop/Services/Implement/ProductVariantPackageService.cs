using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantPackageService : IProductVariantPackageService
    {
        private readonly IProductVariantPackageRepository _productVariantPackageRepository;        

        public ProductVariantPackageService(IProductVariantPackageRepository ProductVariantPackageRepository)
        {
            _productVariantPackageRepository = ProductVariantPackageRepository;            
        }

        public Task<ProductVariantPackage> CreateProductVariantPackage(ProductVariantPackage ProductVariantPackage)
        {
            return _productVariantPackageRepository.CreateProductVariantPackage(ProductVariantPackage);
        }

        public bool DeleteProductVariantPackage(Guid packageId, Guid variantId)
        {
            return _productVariantPackageRepository.DeleteProductVariantPackage(packageId, variantId);
        }

        public IEnumerable<ProductVariantPackage> GetProductVariantPackages()
        {
            return _productVariantPackageRepository.GetProductVariantPackages();
        }

        public ProductVariantPackage? GetProductVariantPackage(Guid packageId, Guid variantId)
        {
            return _productVariantPackageRepository.GetProductVariantPackage(packageId, variantId);
        }

        public bool UpdateProductVariantPackage(ProductVariantPackage ProductVariantPackage)
        {
            return _productVariantPackageRepository.UpdateProductVariantPackage(ProductVariantPackage);
        }        
    }
}

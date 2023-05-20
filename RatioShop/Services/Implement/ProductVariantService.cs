using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _ProductVariantRepository;

        public ProductVariantService(IProductVariantRepository ProductVariantRepository)
        {
            _ProductVariantRepository = ProductVariantRepository;
        }

        public Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant)
        {
            ProductVariant.CreatedDate = DateTime.UtcNow;
            ProductVariant.ModifiedDate = DateTime.UtcNow;
            return _ProductVariantRepository.CreateProductVariant(ProductVariant);
        }

        public bool DeleteProductVariant(int id)
        {
            return _ProductVariantRepository.DeleteProductVariant(id);
        }

        public IEnumerable<ProductVariant> GetProductVariants()
        {
            return _ProductVariantRepository.GetProductVariants();
        }

        public ProductVariant? GetProductVariant(int id)
        {
            return _ProductVariantRepository.GetProductVariant(id);
        }

        public bool UpdateProductVariant(ProductVariant ProductVariant)
        {
            ProductVariant.ModifiedDate = DateTime.UtcNow;
            return _ProductVariantRepository.UpdateProductVariant(ProductVariant);
        }
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductVariantRepository
    {
        IEnumerable<ProductVariant> GetProductVariants();
        IQueryable<ProductVariant> GetProductVariantsByProductId(Guid productId, bool isIncludeDeletedVariant);
        ProductVariant? GetProductVariant(string id);
        Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant);
        bool UpdateProductVariant(ProductVariant ProductVariant);        
        bool DeleteProductVariant(string id, bool isDeepDelete = false);
    }
}

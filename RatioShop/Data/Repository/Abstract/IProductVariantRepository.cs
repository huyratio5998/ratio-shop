using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductVariantRepository
    {
        IEnumerable<ProductVariant> GetProductVariants();
        ProductVariant? GetProductVariant(int id);
        Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant);
        bool UpdateProductVariant(ProductVariant ProductVariant);        
        bool DeleteProductVariant(int id);
    }
}

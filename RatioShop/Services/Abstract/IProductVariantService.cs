using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantService
    {
        IEnumerable<ProductVariant> GetProductVariants();
        ProductVariant? GetProductVariant(int id);
        Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant);
        bool UpdateProductVariant(ProductVariant ProductVariant);
        bool DeleteProductVariant(int id);
    }
}

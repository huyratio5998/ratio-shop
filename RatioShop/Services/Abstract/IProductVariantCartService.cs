using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantCartService
    {
        IEnumerable<ProductVariantCart> GetProductVariantCarts();
        ProductVariantCart? GetProductVariantCart(int id);
        Task<ProductVariantCart> CreateProductVariantCart(ProductVariantCart ProductVariantCart);
        bool UpdateProductVariantCart(ProductVariantCart ProductVariantCart);
        bool DeleteProductVariantCart(int id);
    }
}

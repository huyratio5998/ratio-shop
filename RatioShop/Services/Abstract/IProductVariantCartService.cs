using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.Cart;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantCartService
    {
        IQueryable<ProductVariantCart> GetProductVariantCarts();
        ProductVariantCart? GetProductVariantCart(int id);
        Task<ProductVariantCart> CreateProductVariantCart(ProductVariantCart ProductVariantCart);
        bool UpdateProductVariantCart(ProductVariantCart ProductVariantCart);
        bool DeleteProductVariantCart(string id);

        // addition logic
        bool UpdateStockItemsInCart(Guid cartId, Guid variantId, List<CartStockItem> stockItems);
        bool UpdateStockItemsInVariantCart(ProductVariantCart? variantCarts, List<CartStockItem> stockItems);
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.CartViewModel;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantService
    {
        IEnumerable<ProductVariant> GetProductVariants();
        IEnumerable<ProductVariant> GetProductVariantsByProductId(Guid productId);
        ProductVariant? GetProductVariant(string id);
        Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant);
        bool UpdateProductVariant(ProductVariant ProductVariant);
        bool DeleteProductVariant(string id);

        //Addition logic
        bool ReduceProductVariantNumber(Guid variantId, int reduceNumber, List<CartStockItem>? cartStockItems);
        bool RevertProductVariantNumber(Guid variantId, List<CartStockItem>? cartStockItems);
        bool UpdateProductVariantNumber(Guid variantId);
        bool UpdateProductVariantNumber(ProductVariant productVariant);
        List<ProductVariantStock> GetVariantStocksByStockId(int stockId);
        List<ProductVariantStock> GetVariantStocksOrderByStocks(List<Stock> stocks);
    }
}

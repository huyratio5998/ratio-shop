using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.Cart;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantStockService
    {
        IEnumerable<ProductVariantStock> GetProductVariantStocks();
        IEnumerable<ProductVariantStock> GetProductVariantStocksByVariantId(Guid productVariantId);
        ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId);
        Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock);
        bool CreateOrUpdateProductVariantStock(bool isCreateNew, Guid productVariantId, List<ProductVariantStockViewModel> productVariantStockViewModels);        
        bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock);
        bool DeleteProductVariantStock(int StockId, Guid ProductVariantId);

        // addition logic
        bool ReduceProductVariantStockByNumber(Guid variantId, List<CartStockItem>? cartStockItems);
        bool RevertProductVariantStockByNumber(Guid variantId, List<CartStockItem>? cartStockItems);        
    }
}

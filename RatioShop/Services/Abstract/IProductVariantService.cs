using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantService
    {
        IEnumerable<ProductVariant> GetProductVariants();
        IEnumerable<ProductVariant> GetProductVariantsByProductId(Guid productId, bool isIncludeDeletedVariant = false);
        ProductVariant? GetProductVariant(string id, bool includeProduct = true);
        Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant);
        bool UpdateProductVariant(ProductVariant ProductVariant);
        bool DeleteProductVariant(string id, bool isDeepDelete = false);

        //Addition logic
        bool ReduceProductVariantNumber(Guid variantId, int reduceNumber, List<CartStockItem>? cartStockItems);
        bool RevertProductVariantNumber(Guid variantId, List<CartStockItem>? cartStockItems);
        bool UpdateProductVariantNumber(Guid variantId);
        bool UpdateProductVariantNumber(ProductVariant productVariant);
        List<ProductVariantStock> GetVariantStocksByStockId(int stockId);
        List<ProductVariantStock> GetVariantStocksOrderByStocks(List<Stock> stocks);
        ListProductVariantViewModel GetListVariants(BaseSearchRequest args);
    }
}

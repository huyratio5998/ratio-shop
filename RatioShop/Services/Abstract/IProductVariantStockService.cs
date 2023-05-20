using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IProductVariantStockService
    {
        IEnumerable<ProductVariantStock> GetProductVariantStocks();
        ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId);
        Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock);
        bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock);
        bool DeleteProductVariantStock(int StockId, Guid ProductVariantId);
    }
}

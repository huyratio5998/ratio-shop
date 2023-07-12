using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductVariantStockRepository
    {
        IQueryable<ProductVariantStock> GetProductVariantStocks();
        ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId);
        Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock);
        bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock);        
        bool DeleteProductVariantStock(int StockId, Guid ProductVariantId);
    }
}

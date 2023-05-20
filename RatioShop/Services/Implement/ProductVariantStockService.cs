using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantStockService : IProductVariantStockService
    {
        private readonly IProductVariantStockRepository _ProductVariantStockRepository;

        public ProductVariantStockService(IProductVariantStockRepository ProductVariantStockRepository)
        {
            _ProductVariantStockRepository = ProductVariantStockRepository;
        }

        public Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            return _ProductVariantStockRepository.CreateProductVariantStock(ProductVariantStock);
        }

        public bool DeleteProductVariantStock(int StockId, Guid ProductVariantId)
        {
            return _ProductVariantStockRepository.DeleteProductVariantStock(StockId, ProductVariantId);
        }

        public IEnumerable<ProductVariantStock> GetProductVariantStocks()
        {
            return _ProductVariantStockRepository.GetProductVariantStocks();
        }

        public ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId)
        {
            return _ProductVariantStockRepository.GetProductVariantStock(StockId, ProductVariantId);
        }

        public bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            return _ProductVariantStockRepository.UpdateProductVariantStock(ProductVariantStock);
        }
    }
}

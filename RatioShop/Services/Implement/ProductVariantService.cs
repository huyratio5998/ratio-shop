using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;

        private readonly IProductVariantStockService _productVariantStockService;

        public ProductVariantService(IProductVariantRepository ProductVariantRepository, IProductRepository productRepository, IProductVariantStockService productVariantStockService)
        {
            _productVariantRepository = ProductVariantRepository;
            _productRepository = productRepository;
            _productVariantStockService = productVariantStockService;
        }

        public Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant)
        {
            ProductVariant.CreatedDate = DateTime.UtcNow;
            ProductVariant.ModifiedDate = DateTime.UtcNow;
            return _productVariantRepository.CreateProductVariant(ProductVariant);
        }

        public bool DeleteProductVariant(string id)
        {
            return _productVariantRepository.DeleteProductVariant(id);
        }

        public IEnumerable<ProductVariant> GetProductVariants()
        {
            return _productVariantRepository.GetProductVariants();
        }

        public IEnumerable<ProductVariant> GetProductVariantsByProductId(Guid productId)
        {
            if(productId == null || productId == Guid.Empty) return Enumerable.Empty<ProductVariant>();

            return _productVariantRepository.GetProductVariantsByProductId(productId);
        }

        public ProductVariant? GetProductVariant(string id)
        {
            var productVariant = _productVariantRepository.GetProductVariant(id);
            if(productVariant == null) return null;

            productVariant.Product = _productRepository.GetProduct(productVariant.ProductId).Product;

            return productVariant;
        }

        public bool UpdateProductVariant(ProductVariant ProductVariant)
        {
            ProductVariant.ModifiedDate = DateTime.UtcNow;
            return _productVariantRepository.UpdateProductVariant(ProductVariant);
        }

        /// <summary>
        /// Update product variant number = sum product variant number in stock.
        /// </summary>
        /// <param name="variantId"></param>
        /// <returns></returns>
        public bool UpdateProductVariantNumber(Guid variantId)
        {            
            if(variantId == Guid.Empty) return false;

            var inStockNumber = _productVariantStockService.GetProductVariantStocksByVariantId(variantId).Sum(x=>x.ProductNumber);
            var productVariant = GetProductVariant(variantId.ToString());
            if(productVariant == null) return false;

            productVariant.Number = inStockNumber;
            return UpdateProductVariant(productVariant);            
        }
        public bool UpdateProductVariantNumber(ProductVariant productVariant)
        {
            if(productVariant == null) return false;

            var inStockNumber = _productVariantStockService.GetProductVariantStocksByVariantId(productVariant.Id)?.Sum(x => x.ProductNumber);            

            productVariant.Number = inStockNumber;
            return UpdateProductVariant(productVariant);
        }

        public bool ReduceProductVariantNumber(Guid variantId, int numberReduce, List<CartStockItem>? cartStockItems)
        {
            var variant = _productVariantRepository.GetProductVariant(variantId.ToString());
            if (variant == null || variant.Number < numberReduce) return false;

            // reduce on productVariantStock table
            var productVariantStockSuccess = _productVariantStockService.ReduceProductVariantStockByNumber(variantId, cartStockItems);
            if (!productVariantStockSuccess) return false;

            // reduce on product variant table            
            return UpdateProductVariantNumber(variant);            
        }

        public bool RevertProductVariantNumber(Guid variantId, List<CartStockItem>? cartStockItems)
        {
            var revertOnStock = _productVariantStockService.RevertProductVariantStockByNumber(variantId, cartStockItems);
            if (!revertOnStock) return false;

            return UpdateProductVariantNumber(variantId);
        }

        public List<ProductVariantStock> GetVariantStocksByStockId(int stockId)
        {
            return _productVariantStockService.GetProductVariantStocks().AsQueryable().Where(x=>x.StockId == stockId).ToList();
        }

        public List<ProductVariantStock> GetVariantStocksOrderByStocks(List<Stock> stocks)
        {
            var result = new List<ProductVariantStock>();
            if(stocks == null || stocks.Count == 0) return result;

            foreach (var item in stocks)
            {
                result.AddRange(GetVariantStocksByStockId(item.Id));
            }
            return result;
        }
    }
}

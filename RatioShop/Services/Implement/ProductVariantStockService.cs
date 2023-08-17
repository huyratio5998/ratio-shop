using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantStockService : IProductVariantStockService
    {
        private readonly IProductVariantStockRepository _productVariantStockRepository;

        public ProductVariantStockService(IProductVariantStockRepository ProductVariantStockRepository)
        {
            _productVariantStockRepository = ProductVariantStockRepository;
        }

        public Task<ProductVariantStock> CreateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            return _productVariantStockRepository.CreateProductVariantStock(ProductVariantStock);
        }

        public bool DeleteProductVariantStock(int StockId, Guid ProductVariantId)
        {
            return _productVariantStockRepository.DeleteProductVariantStock(StockId, ProductVariantId);
        }

        public IEnumerable<ProductVariantStock> GetProductVariantStocks()
        {
            return _productVariantStockRepository.GetProductVariantStocks();
        }

        public ProductVariantStock? GetProductVariantStock(int StockId, Guid ProductVariantId)
        {
            return _productVariantStockRepository.GetProductVariantStock(StockId, ProductVariantId);
        }

        public bool UpdateProductVariantStock(ProductVariantStock ProductVariantStock)
        {
            return _productVariantStockRepository.UpdateProductVariantStock(ProductVariantStock);
        }

        public IEnumerable<ProductVariantStock> GetProductVariantStocksByVariantId(Guid productVariantId)
        {
            return _productVariantStockRepository.GetProductVariantStocks().Where(x => x.ProductVariantId == productVariantId);
        }

        public bool CreateOrUpdateProductVariantStock(bool isCreateNew, Guid productVariantId, List<ProductVariantStockViewModel> productVariantStockViewModels)
        {
            // add new
            if (isCreateNew)
            {
                foreach (var item in productVariantStockViewModels)
                {
                    _productVariantStockRepository.CreateProductVariantStock(new ProductVariantStock() { ProductNumber = item.ProductNumber, ProductVariantId = productVariantId, StockId = item.StockId });
                }
            }
            else
            {
                // update - delete
                var oldItems = _productVariantStockRepository.GetProductVariantStocks().Where(x => x.ProductVariantId == productVariantId).ToList();
                foreach (var item in oldItems)
                {
                    var newItem = productVariantStockViewModels.FirstOrDefault(x => x.StockId == item.StockId);
                    if (newItem != null) _productVariantStockRepository.UpdateProductVariantStock(new ProductVariantStock() { ProductNumber = newItem.ProductNumber, ProductVariantId = productVariantId, StockId = newItem.StockId });
                    else _productVariantStockRepository.DeleteProductVariantStock(item.StockId, productVariantId);
                }

                // add new
                var newItems = productVariantStockViewModels.Where(x => !oldItems.Any(y => y.StockId == x.StockId));
                if (newItems != null && newItems.Any())
                {
                    foreach (var item in newItems)
                    {
                        _productVariantStockRepository.CreateProductVariantStock(new ProductVariantStock() { ProductNumber = item.ProductNumber, ProductVariantId = productVariantId, StockId = item.StockId });
                    }
                }

            }
            return true;
        }

        public bool ReduceProductVariantStockByNumber(Guid variantId, List<CartStockItem>? cartStockItems)
        {
            if (cartStockItems == null || cartStockItems.Count == 0) return false;

            var reduce = cartStockItems.Sum(x => x.ItemNumber);
            var items = _productVariantStockRepository.GetProductVariantStocks().Where(x => x.ProductVariantId == variantId);
            if (items == null || !items.Any() || reduce > items.Sum(x => x.ProductNumber)) return false;

            foreach (var item in cartStockItems)
            {
                var productVariantStock = GetProductVariantStock(item.StockId, variantId);
                if (productVariantStock == null) return false;
                if (item.ItemNumber > productVariantStock.ProductNumber) return false;

                productVariantStock.ProductNumber -= item.ItemNumber;
                var updateVariantStockStatus = UpdateProductVariantStock(productVariantStock);
                if (!updateVariantStockStatus) return false;
            }

            return true;
        }

        public bool RevertProductVariantStockByNumber(Guid variantId, List<CartStockItem>? cartStockItems)
        {
            if (variantId == Guid.Empty) return false;
            if (cartStockItems == null || !cartStockItems.Any()) return true;

            foreach (var cartStockItem in cartStockItems)
            {
                var productVariantStock = GetProductVariantStock(cartStockItem.StockId, variantId);
                if (productVariantStock == null) return false;

                productVariantStock.ProductNumber += cartStockItem.ItemNumber;
                var updateVariantStockStatus = UpdateProductVariantStock(productVariantStock);
                if (!updateVariantStockStatus) return false;
            }

            return true;
        }
    }
}

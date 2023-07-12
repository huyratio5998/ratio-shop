using Newtonsoft.Json;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.Cart;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantCartService : IProductVariantCartService
    {
        private readonly IProductVariantCartRepository _ProductVariantCartRepository;

        public ProductVariantCartService(IProductVariantCartRepository ProductVariantCartRepository)
        {
            _ProductVariantCartRepository = ProductVariantCartRepository;
        }

        public Task<ProductVariantCart> CreateProductVariantCart(ProductVariantCart ProductVariantCart)
        {
            ProductVariantCart.CreatedDate = DateTime.UtcNow;
            ProductVariantCart.ModifiedDate = DateTime.UtcNow;
            return _ProductVariantCartRepository.CreateProductVariantCart(ProductVariantCart);
        }

        public bool DeleteProductVariantCart(string id)
        {
            return _ProductVariantCartRepository.DeleteProductVariantCart(id);
        }

        public IQueryable<ProductVariantCart> GetProductVariantCarts()
        {
            return _ProductVariantCartRepository.GetProductVariantCarts();
        }

        public ProductVariantCart? GetProductVariantCart(int id)
        {
            return _ProductVariantCartRepository.GetProductVariantCart(id);
        }

        public bool UpdateProductVariantCart(ProductVariantCart ProductVariantCart)
        {
            ProductVariantCart.ModifiedDate = DateTime.UtcNow;
            return _ProductVariantCartRepository.UpdateProductVariantCart(ProductVariantCart);
        }

        public bool UpdateStockItemsInCart(Guid cartId, Guid variantId, List<CartStockItem> stockItems)
        {
            if (cartId == Guid.Empty || variantId == Guid.Empty) return false;

            var variantCarts = GetProductVariantCarts().FirstOrDefault(x => x.CartId == cartId && x.ProductVariantId == variantId);
            if(variantCarts == null) return false;

            variantCarts.StockItems = JsonConvert.SerializeObject(stockItems);
            return UpdateProductVariantCart(variantCarts);
        }

        public bool UpdateStockItemsInVariantCart(ProductVariantCart? variantCarts, List<CartStockItem> stockItems)
        {
            if(variantCarts == null) return false;

            variantCarts.StockItems = JsonConvert.SerializeObject(stockItems);
            return UpdateProductVariantCart(variantCarts);
        }
    }
}

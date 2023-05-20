using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
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

        public bool DeleteProductVariantCart(int id)
        {
            return _ProductVariantCartRepository.DeleteProductVariantCart(id);
        }

        public IEnumerable<ProductVariantCart> GetProductVariantCarts()
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
    }
}

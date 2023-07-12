using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductVariantCartRepository
    {
        IQueryable<ProductVariantCart> GetProductVariantCarts();
        ProductVariantCart? GetProductVariantCart(int id);
        Task<ProductVariantCart> CreateProductVariantCart(ProductVariantCart ProductVariantCart);
        bool UpdateProductVariantCart(ProductVariantCart ProductVariantCart);        
        bool DeleteProductVariantCart(string id);
    }
}

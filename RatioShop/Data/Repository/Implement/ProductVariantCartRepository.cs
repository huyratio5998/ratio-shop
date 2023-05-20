using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductVariantCartRepository : BaseProductRepository<ProductVariantCart>, IProductVariantCartRepository
    {
        public ProductVariantCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProductVariantCart> CreateProductVariantCart(ProductVariantCart ProductVariantCart)
        {
            return await Create(ProductVariantCart);
        }

        public bool DeleteProductVariantCart(int id)
        {
            return Delete(id);
        }

        public IEnumerable<ProductVariantCart> GetProductVariantCarts()
        {
            return GetAll();
        }

        public ProductVariantCart? GetProductVariantCart(int id)
        {
            return GetById(id);
        }

        public bool UpdateProductVariantCart(ProductVariantCart ProductVariantCart)
        {
            return Update(ProductVariantCart);
        }
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductVariantRepository : BaseProductRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant)
        {
            return await Create(ProductVariant);
        }

        public bool DeleteProductVariant(int id)
        {
            return Delete(id);
        }

        public IEnumerable<ProductVariant> GetProductVariants()
        {
            return GetAll();
        }

        public ProductVariant? GetProductVariant(int id)
        {
            return GetById(id);
        }

        public bool UpdateProductVariant(ProductVariant ProductVariant)
        {
            return Update(ProductVariant);
        }
    }
}

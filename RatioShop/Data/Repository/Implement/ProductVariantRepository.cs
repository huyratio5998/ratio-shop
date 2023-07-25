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

        public bool DeleteProductVariant(string id, bool isDeepDelete = false)
        {
            if (isDeepDelete) return Delete(id);
            else
            {
                var productVariant = GetProductVariant(id);
                if (productVariant == null) return false;

                productVariant.IsDelete = true;
                return Update(productVariant);
            }
        }

        public IEnumerable<ProductVariant> GetProductVariants()
        {
            return GetAll();
        }

        public IQueryable<ProductVariant> GetProductVariantsByProductId(Guid productId, bool isIncludeDeletedVariant)
        {
            if (isIncludeDeletedVariant)
                return GetAll().Where(x => x.ProductId.ToString().ToLower().Equals(productId.ToString().ToLower())).OrderBy(x => x.Price);
            else
                return GetAll().Where(x => !x.IsDelete && x.ProductId.ToString().ToLower().Equals(productId.ToString().ToLower())).OrderBy(x => x.Price);
        }

        public ProductVariant? GetProductVariant(string id)
        {
            return GetById(id);
        }

        public bool UpdateProductVariant(ProductVariant productVariant)
        {
            return Update(productVariant);
        }
    }
}

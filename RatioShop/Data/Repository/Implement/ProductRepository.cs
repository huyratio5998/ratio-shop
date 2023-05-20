using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Enums;
using RatioShop.Helpers.QueryableHelpers;

namespace RatioShop.Data.Repository.Implement
{
    public class ProductRepository : BaseProductRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddProduct(Product product)
        {
            var entity = await Create(product);
            return entity != null;
        }

        public bool DeleteProduct(Guid productId)
        {
            return Delete(productId.ToString());
        }

        public ProductViewModel GetProduct(Guid productId)
        {
            var product = GetById(productId.ToString().ToLower());
            if (product == null) return new ProductViewModel();

            var result = new ProductViewModel(product);
            result.ProductImageName = product.ProductImage;

            return result;
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            return GetAll().Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });
        }

        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize)
        {
            return GetAll(pageNumber, pageSize).Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });
        }

        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
        {
            var sortedProducts = SortedProducts(sortBy);

            return sortedProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });
        }

        public bool UpdateProduct(Product product)
        {
            if (product == null) return false;

            return Update(product);
        }

        private IQueryable<Product> SortedProducts(string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "default":
                    return GetAll().OrderByDescending(nameof(Product.CreatedDate));
                case "oldest":
                    return GetAll().OrderBy(nameof(Product.CreatedDate));
                case "name":
                    return GetAll().OrderBy(nameof(Product.Name));
                case "recentupdate":
                    return GetAll().OrderByDescending(nameof(Product.ModifiedDate));
                default: return GetAll().OrderByDescending(nameof(Product.CreatedDate));
            }
        }
    }
}

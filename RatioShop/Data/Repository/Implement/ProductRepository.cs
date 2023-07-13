using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
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

        public IQueryable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize)
        {
            var sortedProducts = SortedProducts(sortBy);

            return sortedProducts
                .Take(pageNumber * pageSize)
                .Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });
        }

        public ListProductViewModel GetAllListProducts(string searchText, string sortBy, int pageNumber, int pageSize)
        {
            var searchProducts = GetAll()
                .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var fullSearchTextResult = searchProducts.Where(x => x.Code.ToLower().Contains(searchText)
                                || x.Name.ToLower().Contains(searchText)
                                || x.ProductFriendlyName.ToLower().Contains(searchText));

                if (fullSearchTextResult.Count() == 0)
                {
                    var predicate = PredicateBuilder.False<Product>();

                    var listSearchText = searchText.Trim().ToLower().Split(" ").Select(x => x.Trim()).ToList();
                    if (listSearchText != null && listSearchText.Any())
                    {
                        foreach (var text in listSearchText)
                        {
                            predicate = predicate.Or(x => x.Code.ToLower().Contains(text)
                                    || x.Name.ToLower().Contains(text)
                                    || x.ProductFriendlyName.ToLower().Contains(text));
                        }
                    }
                    searchProducts = searchProducts.Where(predicate);
                }
                else searchProducts = fullSearchTextResult;
            }

            var sortedProducts = searchProducts.SortedProducts(sortBy);

            var totalMatch = sortedProducts.Count();
            var pagingResults = sortedProducts                
                .Take(pageNumber * pageSize)
                .Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });

            var result = new ListProductViewModel
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalCount = totalMatch,
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / pageSize),
                Products = pagingResults
            };

            return result;
        }

        public ListProductViewModel GetListProducts(string searchText, string sortBy, int pageNumber, int pageSize)
        {
            var searchProducts = GetAll()
                .Where(x => !x.IsDelete);            

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var fullSearchTextResult = searchProducts.Where(x => x.Code.ToLower().Contains(searchText)
                                || x.Name.ToLower().Contains(searchText)
                                || x.ProductFriendlyName.ToLower().Contains(searchText));

                if (fullSearchTextResult.Count() == 0)
                {
                    var predicate = PredicateBuilder.False<Product>();

                    var listSearchText = searchText.Trim().ToLower().Split(" ").Select(x => x.Trim()).ToList();
                    if (listSearchText != null && listSearchText.Any())
                    {
                        foreach (var text in listSearchText)
                        {
                            predicate = predicate.Or(x => x.Code.ToLower().Contains(text)
                                    || x.Name.ToLower().Contains(text)
                                    || x.ProductFriendlyName.ToLower().Contains(text));
                        }
                    }
                    searchProducts = searchProducts.Where(predicate);
                }
                else searchProducts = fullSearchTextResult;
            }

            var sortedProducts = searchProducts.SortedProducts(sortBy);

            var totalMatch = sortedProducts.Count();
            var pagingResults = sortedProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage });

            var result = new ListProductViewModel
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalCount = totalMatch,
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / pageSize),
                Products = pagingResults
            };

            return result;
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

        public IQueryable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
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

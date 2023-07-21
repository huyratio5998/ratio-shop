using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;

namespace RatioShop.Services.Abstract
{
    public interface IProductService
    {
        public ProductViewModel GetProduct(Guid productId, bool getAdditionInfo = true);
        public void GetProductRelatedInformation(ProductViewModel product);
        public IEnumerable<ProductViewModel> GetProducts();
        public List<ProductViewModel> GetProductsRelatedInformation(List<ProductViewModel> products);
        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize);
        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize);
        public IEnumerable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize);
        // get product on public page
        public IEnumerable<ProductViewModel> GetProductsAvailable();
        public IEnumerable<ProductViewModel> GetProductsAvailable(int pageNumber, int pageSize);
        public IEnumerable<ProductViewModel> GetProductsAvailable(string sortBy, int pageNumber, int pageSize);
        public Task<bool> AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public bool DeleteProduct(Guid productId);
        // Addition logic                
        public ListProductViewModel GetListProducts(ProductSearchRequest request);
        public Dictionary<decimal, decimal?> GetProductPriceFilter(decimal range, int maxDepthFilter);
        public IEnumerable<ProductViewModel> GetRelatedProducts(ProductViewModel product, int maxNumber = 8);

    }
}

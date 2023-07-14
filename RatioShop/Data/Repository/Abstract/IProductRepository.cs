using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        public ProductViewModel GetProduct(Guid productId);
        public IEnumerable<ProductViewModel> GetProducts();
        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize);
        public IQueryable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize);
        public IQueryable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize);        
        public Task<bool> AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public bool DeleteProduct(Guid productId);                
    }
}

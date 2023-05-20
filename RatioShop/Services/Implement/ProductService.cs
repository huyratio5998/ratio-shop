using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;        

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;            
        }

        public async Task<bool> AddProduct(Product product)
        {
            product.CreatedDate = DateTime.UtcNow;
            product.ModifiedDate = DateTime.UtcNow;

            return await productRepository.AddProduct(product);
        }

        public bool DeleteProduct(Guid productId)
        {
            return productRepository.DeleteProduct(productId);
        }

        public ProductViewModel GetProduct(Guid productId)
        {
            return productRepository.GetProduct(productId);
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            return productRepository.GetProducts();
        }

        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize)
        {
            return productRepository.GetProducts(pageNumber,pageSize);            
        }

        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(sortBy)) return GetProducts(pageNumber, pageSize);

            return productRepository.GetProducts(sortBy, pageNumber, pageSize);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable()
        {
            return productRepository.GetProducts().Where(p=> !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(int pageNumber, int pageSize)
        {
            return productRepository.GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(string sortBy, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(sortBy)) return GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);

            return productRepository.GetProducts(sortBy, pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public bool UpdateProduct(Product product)
        {
            product.ModifiedDate = DateTime.UtcNow;

            return productRepository.UpdateProduct(product);
        }
    }
}

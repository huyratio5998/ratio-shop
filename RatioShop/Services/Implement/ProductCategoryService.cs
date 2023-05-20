using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _ProductCategoryRepository;

        public ProductCategoryService(IProductCategoryRepository ProductCategoryRepository)
        {
            _ProductCategoryRepository = ProductCategoryRepository;
        }

        public Task<ProductCategory> CreateProductCategory(ProductCategory ProductCategory)
        {
            return _ProductCategoryRepository.CreateProductCategory(ProductCategory);
        }

        public bool DeleteProductCategory(int CategoryId, Guid ProductId)
        {
            return _ProductCategoryRepository.DeleteProductCategory(CategoryId, ProductId);
        }

        public IEnumerable<ProductCategory> GetProductCategorys()
        {
            return _ProductCategoryRepository.GetProductCategorys();
        }

        public ProductCategory? GetProductCategory(int CategoryId, Guid ProductId)
        {
            return _ProductCategoryRepository.GetProductCategory(CategoryId, ProductId);
        }

        public bool UpdateProductCategory(ProductCategory ProductCategory)
        {
            return _ProductCategoryRepository.UpdateProductCategory(ProductCategory);
        }
    }
}

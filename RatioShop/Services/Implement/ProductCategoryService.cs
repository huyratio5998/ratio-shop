using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _ProductCategoryRepository;
        private readonly ICategoryService _categoryService;

        public ProductCategoryService(IProductCategoryRepository ProductCategoryRepository, ICategoryService categoryService)
        {
            _ProductCategoryRepository = ProductCategoryRepository;
            _categoryService = categoryService;
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

        public IEnumerable<Category> GetCategorysByProductId(Guid productId)
        {
            return _ProductCategoryRepository.GetProductCategorys()
                .Join(_categoryService.GetCategories(),
                x => x.CategoryId,
                y => y.Id, (x, y) => new { productCategory = x, category = y })
                .Where(z => z.productCategory.ProductId == productId)
                .Select(x => x.category);
        }
    }
}

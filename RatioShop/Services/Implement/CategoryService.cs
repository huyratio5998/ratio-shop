using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IProductCategoryRepository productCategoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public Task<Category> CreateCategory(Category category)
        {
            category.CreatedDate = DateTime.UtcNow;
            category.ModifiedDate = DateTime.UtcNow;
            return _categoryRepository.CreateCategory(category);
        }

        public bool DeleteCategory(int id)
        {
            return _categoryRepository.DeleteCategory(id);
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.GetCategories();
        }

        public Category? GetCategory(int id)
        {
            return _categoryRepository.GetCategory(id);
        }

        public IEnumerable<Category> GetCategorysByProductId(Guid productId)
        {
            return _categoryRepository.GetCategories()
                .Join(_productCategoryRepository.GetProductCategorys(),
                x => x.Id,
                y => y.CategoryId,
                (x, y) => new { category = x, productCategories = y })
                .Where(z => z.productCategories.ProductId.ToString().ToLower().Equals(productId.ToString().ToLower()))
                .Select(x=>x.category);
        }

        public bool UpdateCategory(Category category)
        {            
            category.ModifiedDate = DateTime.UtcNow;
            return _categoryRepository.UpdateCategory(category);
        }
    }
}

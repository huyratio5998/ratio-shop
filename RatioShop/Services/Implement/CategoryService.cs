using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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

        public bool UpdateCategory(Category category)
        {            
            category.ModifiedDate = DateTime.UtcNow;
            return _categoryRepository.UpdateCategory(category);
        }
    }
}

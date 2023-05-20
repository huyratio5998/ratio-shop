using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
        Category? GetCategory(int id);
        Task<Category> CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int id);
    }
}

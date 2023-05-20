using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetCategories();
        Category? GetCategory(int id);
        Task<Category> CreateCategory(Category category);
        bool UpdateCategory(Category category);        
        bool DeleteCategory(int id);
    }
}

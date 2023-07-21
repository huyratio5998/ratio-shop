using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategories();
        Category? GetCategory(int id);
        Task<Category> CreateCategory(Category category);
        bool UpdateCategory(Category category);        
        bool DeleteCategory(int id);

        //
        IQueryable<Category> GetCategoriesWithParentData();
    }
}

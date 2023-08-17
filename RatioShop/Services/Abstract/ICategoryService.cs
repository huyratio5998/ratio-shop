using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
        IEnumerable<Category> GetCategorysByProductId(Guid productId);
        Category? GetCategory(int id);
        Task<Category> CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int id);

        //
        IEnumerable<Category> GetCategoriesWithParentData();
        IEnumerable<Category> GetListCategoriesChildren(int categoryId, bool isIncludeCurrentCategory = false);
    }
}

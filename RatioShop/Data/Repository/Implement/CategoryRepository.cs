using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class CategoryRepository : BaseEntityRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category> CreateCategory(Category category)
        {
            return await Create(category);
        }

        public bool DeleteCategory(int id)
        {
            return Delete(id);
        }

        public IQueryable<Category> GetCategories()
        {
            return GetAll();
        }

        public IQueryable<Category> GetCategoriesWithParentData()
        {
            return _context.Category.AsNoTracking().Include(x => x.ParentCategory).AsQueryable();
        }

        public Category? GetCategory(int id)
        {
            var category = GetById(id);   
            if(category != null && category.ParentId != null) category.ParentCategory = GetById((int)category.ParentId);

            return category;
        }

        public bool UpdateCategory(Category category)
        {
            return Update(category);
        }
    }
}

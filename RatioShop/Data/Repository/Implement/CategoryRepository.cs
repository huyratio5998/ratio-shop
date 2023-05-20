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

        public IEnumerable<Category> GetCategories()
        {
            return GetAll();
        }

        public Category? GetCategory(int id)
        {
            return GetById(id);
        }

        public bool UpdateCategory(Category category)
        {
            return Update(category);
        }
    }
}

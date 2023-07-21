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

        public IEnumerable<Category> GetCategoriesWithParentData()
        {
            return _categoryRepository.GetCategoriesWithParentData();
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
                .Select(x => x.category);
        }

        public bool UpdateCategory(Category category)
        {
            category.ModifiedDate = DateTime.UtcNow;
            return _categoryRepository.UpdateCategory(category);
        }

        public IEnumerable<Category> GetListCategoriesChildren(int categoryId, bool isIncludeCurrentCategory = false)
        {
            if (categoryId == 0) return Enumerable.Empty<Category>();

            var result = new List<Category>();
            if (isIncludeCurrentCategory)
            {
                var currentCategory = GetCategory(categoryId);
                if (currentCategory == null) return Enumerable.Empty<Category>();

                result.Add(currentCategory);
            }

            GetAllCategoryChildrenByParentId(_categoryRepository.GetCategories().ToList(), ref result, categoryId);

            return result;
        }

        private void GetAllCategoryChildrenByParentId(List<Category> categories, ref List<Category> result, int categoryId)
        {
            var childrenCategories = categories.Where(x => x.ParentId == categoryId);
            result.AddRange(childrenCategories);

            if (childrenCategories != null && childrenCategories.Any())
            {
                foreach (var item in childrenCategories)
                {
                    GetAllCategoryChildrenByParentId(categories, ref result, item.Id);
                }
            }
            else
                return;
        }
    }
}

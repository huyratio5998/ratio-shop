using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IProductCategoryService
    {
        IEnumerable<ProductCategory> GetProductCategorys();
        ProductCategory? GetProductCategory(int CategoryId, Guid ProductId);
        Task<ProductCategory> CreateProductCategory(ProductCategory ProductCategory);
        bool UpdateProductCategory(ProductCategory ProductCategory);
        bool DeleteProductCategory(int CategoryId, Guid ProductId);
    }
}

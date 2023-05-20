using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductCategoryRepository
    {
        IEnumerable<ProductCategory> GetProductCategorys();
        ProductCategory? GetProductCategory(int CategoryId, Guid ProductId);
        Task<ProductCategory> CreateProductCategory(ProductCategory ProductCategory);
        bool UpdateProductCategory(ProductCategory ProductCategory);        
        bool DeleteProductCategory(int CategoryId, Guid ProductId);
    }
}

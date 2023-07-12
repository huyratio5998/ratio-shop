using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IProductCategoryRepository
    {
        IQueryable<ProductCategory> GetProductCategorys();
        IQueryable<ProductCategory> GetProductCategorys(int pageIndex, int pageSize);
        ProductCategory? GetProductCategory(int CategoryId, Guid ProductId);
        Task<ProductCategory> CreateProductCategory(ProductCategory ProductCategory);
        bool UpdateProductCategory(ProductCategory ProductCategory);        
        bool DeleteProductCategory(int CategoryId, Guid ProductId);
    }
}

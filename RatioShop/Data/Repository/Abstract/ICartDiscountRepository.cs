using RatioShop.Data.Models;
using System.Linq.Expressions;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICartDiscountRepository
    {
        IQueryable<CartDiscount> GetCartDiscounts();
        CartDiscount? GetCartDiscount(int id);
        Task<CartDiscount> CreateCartDiscount(CartDiscount CartDiscount);
        bool UpdateCartDiscount(CartDiscount CartDiscount);        
        bool DeleteCartDiscount(int id);
        CartDiscount? Find(Expression<Func<CartDiscount,bool>> predicate);
    }
}

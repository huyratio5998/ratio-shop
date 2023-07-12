using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IDiscountRepository
    {
        IQueryable<Discount> GetDiscounts();
        Discount? GetDiscount(int id);
        Task<Discount> CreateDiscount(Discount Discount);
        bool UpdateDiscount(Discount Discount);        
        bool DeleteDiscount(int id);
    }
}

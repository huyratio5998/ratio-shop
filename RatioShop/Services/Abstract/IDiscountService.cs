using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IDiscountService
    {
        IEnumerable<Discount> GetDiscounts();
        Discount? GetDiscount(int id);
        Task<Discount> CreateDiscount(Discount Discount);
        bool UpdateDiscount(Discount Discount);
        bool DeleteDiscount(int id);

        Discount? GetDiscountByCode(string code);
        bool TemporaryDeleteDiscount(int id);


    }
}

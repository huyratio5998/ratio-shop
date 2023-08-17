using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface ICartDiscountService
    {
        IEnumerable<CartDiscount> GetCartDiscounts();
        CartDiscount? GetCartDiscount(int id);
        Task<CartDiscount> CreateCartDiscount(CartDiscount CartDiscount);
        bool UpdateCartDiscount(CartDiscount CartDiscount);
        bool DeleteCartDiscount(int id);      

        bool CheckDiscountAvailable(Guid cartId, Discount discount);
        bool ResetCartDiscount(Guid cartId);
        IEnumerable<CartDiscount> GetCartDiscountsByCartId(Guid cartId);
        IEnumerable<string> GetListCouponApplyByCartId(Guid cartId, bool includeInActiveStatus = false);
        IEnumerable<Discount> GetDicountsByCouponsCode(List<string> coupons);
        CartDiscount? GetCartDiscount(Guid cartId, string coupon);
        bool DeleteCartDiscount(Guid cartId, string coupon);


    }
}

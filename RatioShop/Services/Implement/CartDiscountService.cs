using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Enums;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CartDiscountService : ICartDiscountService
    {
        private readonly ICartDiscountRepository _cartDiscountRepository;
        private readonly IDiscountService _discountService;

        public CartDiscountService(ICartDiscountRepository CartDiscountRepository, IDiscountService discountService)
        {
            _cartDiscountRepository = CartDiscountRepository;
            _discountService = discountService;
        }

        public Task<CartDiscount> CreateCartDiscount(CartDiscount CartDiscount)
        {
            CartDiscount.CreatedDate = DateTime.UtcNow;
            CartDiscount.ModifiedDate = DateTime.UtcNow;
            return _cartDiscountRepository.CreateCartDiscount(CartDiscount);
        }

        public bool DeleteCartDiscount(int id)
        {
            return _cartDiscountRepository.DeleteCartDiscount(id);
        }

        public IEnumerable<CartDiscount> GetCartDiscounts()
        {
            return _cartDiscountRepository.GetCartDiscounts();
        }

        public CartDiscount? GetCartDiscount(int id)
        {
            return _cartDiscountRepository.GetCartDiscount(id);
        }

        public bool UpdateCartDiscount(CartDiscount CartDiscount)
        {
            CartDiscount.ModifiedDate = DateTime.UtcNow;
            return _cartDiscountRepository.UpdateCartDiscount(CartDiscount);
        }

        public bool CheckDiscountAvailable(Guid cartId, Discount newDiscount)
        {
            var currentCartDiscounts = _cartDiscountRepository.GetCartDiscounts().Where(x => x.CartId == cartId);
            var couponUsed = currentCartDiscounts.FirstOrDefault(x => x.DiscountId == newDiscount.Id);
            if (couponUsed == null)
            {
                // only type direct cash => apply immediately
                if (newDiscount.DiscountType == DiscountType.DirectValue) return true;

                // join 2 table
                var cartDiscountAndDiscount = currentCartDiscounts.
                    Join(_discountService.GetDiscounts(), x => x.DiscountId, y => y.Id, (x, y) => new { cartDiscount = x, discount = y });

                if (newDiscount.DiscountType == DiscountType.Freeship)
                {
                    // replace partialFreeship by freeship
                    var currentPartialFreeship = cartDiscountAndDiscount.FirstOrDefault(x => x.discount.DiscountType == DiscountType.PartialFreeship);                    
                    if (currentPartialFreeship != null) _cartDiscountRepository.DeleteCartDiscount(currentPartialFreeship.cartDiscount.Id);
                    return true;
                }else if(newDiscount.DiscountType == DiscountType.PartialFreeship)
                {
                    var freeShipUltimate = cartDiscountAndDiscount.Any(x => x.discount.DiscountType == DiscountType.Freeship);
                    if (freeShipUltimate) return false;
                }
                // check if better => replace
                var couponHasSameTypeApplied = cartDiscountAndDiscount.FirstOrDefault(x => x.discount.DiscountType == newDiscount.DiscountType);

                if (couponHasSameTypeApplied == null) return true;
                if (couponHasSameTypeApplied != null && newDiscount.Value > couponHasSameTypeApplied?.discount.Value)
                {
                    // remove old                    
                    _cartDiscountRepository.DeleteCartDiscount(couponHasSameTypeApplied.cartDiscount.Id);
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<CartDiscount> GetCartDiscountsByCartId(Guid cartId)
        {
            if (cartId == Guid.Empty) return Enumerable.Empty<CartDiscount>();

            return GetCartDiscounts().AsQueryable().Where(x => x.CartId == cartId)
                .Join(_discountService.GetDiscounts(),
                x => x.DiscountId,
                y => y.Id,
                (x, y) => new { cartDiscount = x, discount = y })
                .Where(x => x.discount.Status.Equals(CommonStatus.Discount.Active))
                .Select(x => x.cartDiscount);
        }

        public bool ResetCartDiscount(Guid cartId)
        {
            var items = GetCartDiscountsByCartId(cartId);
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    DeleteCartDiscount(item.Id);
                }
            }
            return true;
        }

        public IEnumerable<string> GetListCouponApplyByCartId(Guid cartId)
        {
            if (cartId == Guid.Empty) return Enumerable.Empty<string>();

            return GetCartDiscounts().AsQueryable().Where(x => x.CartId == cartId)
                .Join(_discountService.GetDiscounts(),
                x => x.DiscountId,
                y => y.Id,
                (x, y) => new { cartDiscount = x, discount = y })
                .Where(x => x.discount.Status.Equals(CommonStatus.Discount.Active))
                .Select(x => x.discount.Code);
        }

        public IEnumerable<Discount> GetDicountsByCouponsCode(List<string> coupons)
        {
            foreach (var couponCode in coupons)
            {
                var discount = _discountService.GetDiscountByCode(couponCode);
                if (discount != null) yield return discount;
            }
        }

        public CartDiscount? GetCartDiscount(Guid cartId, string coupon)
        {
            var cartDiscount = GetCartDiscounts().AsQueryable().Where(x => x.CartId == cartId)
                .Join(_discountService.GetDiscounts(),
                x => x.DiscountId,
                y => y.Id,
                (x, y) => new { cartDiscount = x, discount = y })
                .FirstOrDefault(x => x.discount.Status.Equals(CommonStatus.Discount.Active) && x.discount.Code.Equals(coupon))
                ?.cartDiscount;

            return cartDiscount;
        }

        public bool DeleteCartDiscount(Guid cartId, string coupon)
        {
            var cartDiscountId = GetCartDiscount(cartId, coupon)?.Id;
            if (cartDiscountId == null || cartDiscountId == 0) return false;

            return DeleteCartDiscount((int)cartDiscountId);
        }
    }
}

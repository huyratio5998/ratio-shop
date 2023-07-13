using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Helpers.CookieHelpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartDiscountService _cartDiscountService;
        private readonly IDiscountService _discountService;
        private readonly IProductVariantCartService _productVariantCartService;


        private readonly Guid _anonymousUserID = Guid.Parse(UserTest.UserAnonymousID);
        public CartController(ICartService cartService, IProductVariantCartService productVariantCartService, ICartDiscountService cartDiscountService, IDiscountService discountService)
        {
            _cartService = cartService;
            _productVariantCartService = productVariantCartService;
            _cartDiscountService = cartDiscountService;
            _discountService = discountService;
        }
        [HttpPost]
        [Route("addToCart")]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestViewModel data)
        {
            if (User == null || !User.Identity.IsAuthenticated) return Ok(new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "UnAuthenticated", false, false));

            if (data == null || data.VariantId == Guid.Empty || data.Number <= 0) return BadRequest();

            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var cartId);
            data.CartId = cartId;

            Guid.TryParse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            data.UserId = userId;

            var product = await _cartService.AddToCart(data);

            if (data.CartId == null || data.CartId == Guid.Empty) Response.Cookies.Append(CookieKeys.CartId, product.CartId.ToString(), CookieHelpers.DefaultOptionByDays(14));

            return Ok(product);
        }

        [HttpPost]
        [Route("changeCartItem")]
        public IActionResult ChangeCartItem([FromBody] AddToCartRequestViewModel data)
        {
            if (data == null || data.VariantId == Guid.Empty || data.Number < 0) return BadRequest();

            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var cartId);
            data.CartId = cartId;

            if (data.UserId == null || data.UserId == Guid.Empty) data.UserId = _anonymousUserID;

            var product = _cartService.ChangeCartItem(data);

            return Ok(product);
        }

        [Route("detail")]
        public IActionResult GetCartDetail()
        {
            var cartIdString = Request.Cookies[CookieKeys.CartId]?.ToString();
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(cartIdString))
            {
                // get cat from user account if exist.
                var cart = _cartService.GetCartByUserId(userId);
                if (cart != null)
                {
                    Response.Cookies.Append(CookieKeys.CartId, cart.Id.ToString(), CookieHelpers.DefaultOptionByDays(14));
                    cartIdString = cart.Id.ToString();
                }
            }

            Guid.TryParse(cartIdString, out var cartId);
            if (cartId == Guid.Empty) return Ok(new CartDetailResponsViewModel());

            var cartDetail = _cartService.GetCartDetail(cartId);

            if (cartDetail == null) return BadRequest();
            return Ok(cartDetail);
        }

        [Route("cleanAnonymousCart")]
        public async Task<IActionResult> CleanAnonymousCart()
        {
            var cleanNumber = 0;
            var anonymousCartIds = _cartService.GetCarts().Where(x => x.ShopUserId.Equals(UserTest.UserAnonymousID)).Select(x => x.Id).ToList();
            if (anonymousCartIds == null || !anonymousCartIds.Any()) return Ok($"Clean anonymous cart: {cleanNumber}");

            foreach (var item in anonymousCartIds)
            {
                string cartId = item.ToString();
                _productVariantCartService.DeleteProductVariantCart(cartId);
                _cartService.DeleteCart(cartId);
                cleanNumber++;
            }

            return Ok($"Success clean anonymous cart: {cleanNumber}");
        }

        [Route("applycouponcode")]
        public IActionResult ApplyCouponCode([FromQuery] string couponCode)
        {
            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var cartId);
            if (cartId == Guid.Empty || string.IsNullOrWhiteSpace(couponCode)) return BadRequest();

            var cartDetail = _cartService.GetCart(cartId.ToString());
            var discount = _discountService.GetDiscountByCode(couponCode);
            if (cartDetail == null || discount == null) return NotFound();

            // validate 1 coupon code appreas once each cart.            
            var validation = _cartDiscountService.CheckDiscountAvailable(cartId, discount);
            if (validation) _cartDiscountService.CreateCartDiscount(new Data.Models.CartDiscount() { CartId = cartId, DiscountId = discount.Id });

            return Ok(validation);
        }

        [Route("removecouponcode")]
        public IActionResult RemoveCouponCode([FromQuery] string couponCode)
        {
            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var cartId);
            if (cartId == Guid.Empty || string.IsNullOrWhiteSpace(couponCode)) return BadRequest();

            return Ok(_cartDiscountService.DeleteCartDiscount(cartId, couponCode));
        }
    }
}

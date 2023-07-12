using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ICartService _cartService;
        private readonly IShopUserService _userService;


        public AddressController(IAddressService addressService, ICartService cartService, IShopUserService userService)
        {
            _addressService = addressService;
            _cartService = cartService;
            _userService = userService;
        }

        [Route("getAddress")]
        public IActionResult GetAddressByTypeValue([FromQuery]string type, [FromQuery] string value)
        {            
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value)) return BadRequest();

            var addresses = _addressService.GetAddressesByValueOfType(type, value).Select(x=>new AddressResponseViewModel(x));

            return Ok(addresses);
        }

        [Route("getAddressByType")]
        public IActionResult GetAddressByType([FromQuery] string type)
        {
            if (string.IsNullOrEmpty(type) ) return BadRequest();

            var addresses = _addressService.GetAddressesByType(type);

            return Ok(addresses);
        }

        [Route("updateCartShippingAddress")]
        public IActionResult UpdateCartShippingAddress([FromQuery] string city, [FromQuery] string district, [FromQuery] string addressDetail, [FromQuery] string fullName, [FromQuery] string phoneNumber)
        {
            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var currentCartId);
            
            if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(district) || currentCartId == Guid.Empty || string.IsNullOrWhiteSpace(addressDetail) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber)) return BadRequest();

            var address = _addressService.GetAddressByCityAndDistrict(city, district);
            // update cart shipping address            
            var currentCart = _cartService.GetCart(currentCartId.ToString());
            if (currentCart != null)
            {
                currentCart.AddressId = address?.Id;
                currentCart.AddressDetail = addressDetail;
                currentCart.FullName = fullName;
                currentCart.PhoneNumber = phoneNumber;
                _cartService.UpdateCart(currentCart);

                // update back to user.
                var updateUser = _userService.GetShopUser(currentCart.ShopUserId);
                if (updateUser != null)
                    _userService.UpdateShopUserNewShippingAddressInfor(updateUser, currentCart);
            }

            return Ok(address?.ShippingFee);
        }

        [Route("getShippingFee")]
        public IActionResult GetShippingFeeByAddress([FromQuery] string city, [FromQuery] string district)
        {
            if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(district)) return BadRequest();

            var address = _addressService.GetAddressByCityAndDistrict(city, district);


            return Ok(address?.ShippingFee);
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers;
using RatioShop.Helpers.CookieHelpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Features
{
    [AllowAnonymous]
    public class CartController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IAddressService _addressService;

        public CartController(IMapper mapper, ICartService cartService, IAddressService addressService)
        {
            _mapper = mapper;
            _cartService = cartService;
            _addressService = addressService;
        }

        public IActionResult CartDetail()
        {
            if (User == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Products");
            var result = new CartDetailViewModel();

            // get cart id => should use filter or helper for those logic.
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
            
            if (cartId == Guid.Empty) return View("~/Views/Cart/CartDetail.cshtml", result);

            var cartDetail = _cartService.GetCartDetail(cartId, true, false, true);
            if (cartDetail == null) return View();

            result = _mapper.Map<CartDetailViewModel>(cartDetail);

            result.ListCities = _addressService.GetAddressesByType("Address1").ToList();
            if(cartDetail.ShippingAddressId != null)
            {
                var address = _addressService.GetAddress((int)cartDetail.ShippingAddressId);
                if(address != null)
                {
                    result.ShippingAddress = new AddressResponseViewModel(address);
                    result.ShippingAddress.AddressDetail = cartDetail.ShippingAddressDetail;                    
                    result.ListDistrict = _addressService.GetAddressesByValueOfType("Address1", address.Address1).Select(x=>x.Address2).OrderBy(x=>x).ToList();
                }                
            }            

            return View("~/Views/Cart/CartDetail.cshtml", result);
        }               
    }
}

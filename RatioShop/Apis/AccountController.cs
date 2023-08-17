using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels.User;
using RatioShop.Helpers.CookieHelpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IShopUserService _shopUserService;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        public AccountController(IShopUserService shopUserService, ICartService cartService, IMapper mapper)
        {
            _shopUserService = shopUserService;
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterRequestViewModel request)
        {
            if (request == null) return BadRequest();

            var registerUserResponse = await _shopUserService.RegisterUser(request);
            UpdateUserIdToCart(registerUserResponse.Status);

            return Ok(registerUserResponse);
        }

        [Route("loginOrRegisterPopup")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginOrRegisterPopup([FromQuery] bool isLoginPopup = false)
        {
            var result = await _shopUserService.LoginOrRegisterPopupAction(isLoginPopup);

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginRequestViewModel request)
        {
            if (request == null) return BadRequest();

            var registerUserResponse = await _shopUserService.UserLogin(request);
            UpdateUserIdToCart(registerUserResponse.Status);

            return Ok(registerUserResponse);
        }

        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var logoutResponse = await _shopUserService.UserLogout();
            if (logoutResponse) Response.Cookies.Delete(CookieKeys.CartId);

            return Ok(logoutResponse);
        }

        [Route("authenticated")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAuthenticated()
        {
            try
            {
                var result = true;
                if (User == null || !User.Identity.IsAuthenticated) result = false;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [HttpPost]
        [Route("externalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin([FromForm] string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            return _shopUserService.ExternalLogin(provider, redirectUrl);
        }

        [Route("externalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var result = await _shopUserService.ExternalLoginCallback();
            UpdateUserIdToCart(result.Status);

            if (result.Status.Equals("Success")) return LocalRedirect(returnUrl);
            return RedirectToAction("ExternalLoginFailure", "Login", new { returnUrl });
        }

        [Route("getPersonalInformation")]
        public async Task<IActionResult> GetPersonalInformation()
        {
            if (User == null || !User.Identity.IsAuthenticated) return NotFound();

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            var userInfo = _shopUserService.GetShopUserViewModel(userId);
            var result = _mapper.Map<UserResponseViewModel>(userInfo);            

            return Ok(result);
        }

        [HttpPost]
        [Route("updatePersonalInformation")]        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInformation([FromForm] PersonalInfoRequestViewModel request)
        {
            if (User == null || !User.Identity.IsAuthenticated) return NotFound();
            if (request == null) return BadRequest();

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            var updateStatus = _shopUserService.UpdatePersonalInformation(userId, request);            

            return Ok(updateStatus);
        }

        private void UpdateUserIdToCart(string status)
        {
            if (status.Equals("Success"))
            {
                var cartId = Request.Cookies[CookieKeys.CartId]?.ToString();
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(cartId))
                {
                    bool isUpdateUserId = _cartService.UpdateCartUserByUserId(cartId, userId);
                    // won't update => remove current cart from cookies.
                    if (!isUpdateUserId) Response.Cookies.Delete(CookieKeys.CartId);
                }
                else
                {
                    var cart = _cartService.GetCartByUserId(userId);
                    if (cart != null)
                        Response.Cookies.Append(CookieKeys.CartId, cart.Id.ToString(), CookieHelpers.DefaultOptionByDays(14));
                }
            }
        }
        private void UpdateUserIdToCart(bool isUpdate)
        {
            if (isUpdate)
            {
                var cartId = Request.Cookies[CookieKeys.CartId]?.ToString();
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(cartId))
                {
                    bool isUpdateUserId = _cartService.UpdateCartUserByUserId(cartId, userId);
                    // won't update => remove current cart from cookies.
                    if (!isUpdateUserId) Response.Cookies.Delete(CookieKeys.CartId);
                }
                else
                {
                    var cart = _cartService.GetCartByUserId(userId);
                    if (cart != null)
                        Response.Cookies.Append(CookieKeys.CartId, cart.Id.ToString(), CookieHelpers.DefaultOptionByDays(14));
                }
            }
        }
    }
}

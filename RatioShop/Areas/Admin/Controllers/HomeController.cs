using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Enums;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,Shipper,Employee,ContentEditor")]
    public class HomeController : Controller
    {
        private readonly IShopUserService _userService;

        public HomeController(IShopUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _userService.GetShopUser(userId);
            var roles = await _userService.GetUserRoles(user);            

            ViewBag.UserRole = user != null ? roles : null;

            return View();
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.User;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Features
{
    public class MyAccountController : Controller
    {
        private readonly IShopUserService _shopUserService;
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;


        private const int pageSizeClientDesktopDefault = 6;
        private const int pageSizeClientMobileDefault = 5;

        public MyAccountController(IShopUserService shopUserService, IOrderService orderService, IAddressService addressService)
        {
            _shopUserService = shopUserService;
            _orderService = orderService;
            _addressService = addressService;
        }

        public IActionResult Index(string tab = CommonConstant.MyAccount.PersonalTab, int page = 1)
        {            
            if (User == null || !User.Identity.IsAuthenticated) RedirectToAction("Index", "Home");

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || string.IsNullOrWhiteSpace(userId)) RedirectToAction("Index", "Home");

            ListOrderViewModel orderHistories = null;
            List<string> cities = new List<string>();
            List<string> districts = new List<string>();
            UserViewModel userData = null;
            if (tab.Equals(CommonConstant.MyAccount.OrderHistoryTab, StringComparison.OrdinalIgnoreCase))
            {                
                var totalOrderByUser = _orderService.GetTotalOrderByUserId(userId);
                var orderPageSize = GetDynamicPageSizeForOrderHistory(totalOrderByUser);

                orderHistories = new ListOrderViewModel
                {
                    Orders = _orderService.GetOrderHistoryByUserId(userId, page, orderPageSize)?.ToList(),
                    PageIndex = page,
                    PageSize = orderPageSize,
                    TotalCount = totalOrderByUser,
                    TotalPage = totalOrderByUser == 0 ? 1 : (int)Math.Ceiling((double)totalOrderByUser / orderPageSize),
                };
            }
            else if (tab.Equals(CommonConstant.MyAccount.PersonalTab, StringComparison.OrdinalIgnoreCase))
            {
                userData = _shopUserService.GetShopUserViewModel(userId);
                cities = _addressService.GetAddressesByType("Address1").ToList();
                districts = _addressService.GetAddressesByValueOfType("Address1", userData.User?.Address?.Address1 ?? cities.FirstOrDefault())?.Select(x => x.Address2).ToList();
            }

            var myAccount = new MyAccountViewModel
            {
                SelectedTab = tab,
                UserData = userData,
                OrderHistory = orderHistories,
                ListCities = cities,
                ListDistrict = districts
            };
            return View(myAccount);
        }

        private int GetDynamicPageSizeForOrderHistory(int totalItems)
        {
            var orderPageSize = CommonHelper.GetClientDevice(Request) == Enums.DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var step = 50;
            for(int i = 0; i < totalItems; i+= step)
            {
                if (i >= step) orderPageSize += 5;
            }
            return orderPageSize;            
        }
    }
}

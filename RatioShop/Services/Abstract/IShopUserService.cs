using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.User;

namespace RatioShop.Services.Abstract
{
    public interface IShopUserService
    {
        IEnumerable<ShopUser> GetShopUsers();
        ShopUser? GetShopUser(string id);
        UserViewModel GetShopUserViewModel(string id);
        Task<ShopUser> CreateShopUser(ShopUser ShopUser);
        bool UpdateShopUser(ShopUser ShopUser);
        bool DeleteShopUser(string id);

        // addition logic
        Task<RegisterResponseViewModel> RegisterUser(RegisterRequestViewModel user);
        Task<LoginResponseViewModel> UserLogin(LoginRequestViewModel request);
        Task<bool> UserLogout();
        Task<LoginPopupViewModel> LoginOrRegisterPopupAction(bool isLoginPopup);
        ChallengeResult ExternalLogin(string provider, string redirectUrl);
        Task<LoginResponseViewModel> ExternalLoginCallback();
        bool UpdateShopUserNewShippingAddressInfor(ShopUser ShopUser, string phoneNumber, string fullName, string adderssDetail, int? addressId);
        bool UpdateShopUserNewShippingAddressInfor(ShopUser? shopUser, Cart? cart);
        bool UpdatePersonalInformation(string id, PersonalInfoRequestViewModel personalInfo);

    }
}

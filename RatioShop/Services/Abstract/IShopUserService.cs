using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Areas.Admin.Models.User;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.SearchViewModel;
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
        Task<List<string>>? GetUserRoles(ShopUser user);
        Task<RegisterResponseViewModel> RegisterUser(RegisterRequestViewModel user);
        Task<LoginPopupViewModel> LoginOrRegisterPopupAction(bool isLoginPopup);
        Task<LoginResponseViewModel> UserLogin(LoginRequestViewModel request);
        ChallengeResult ExternalLogin(string provider, string redirectUrl);
        Task<LoginResponseViewModel> ExternalLoginCallback();
        Task<bool> UserLogout();
        bool UpdateShopUserNewShippingAddressInfor(ShopUser ShopUser, string phoneNumber, string fullName, string adderssDetail, int? addressId);
        bool UpdateShopUserNewShippingAddressInfor(ShopUser? shopUser, Cart? cart);
        bool UpdatePersonalInformation(string id, PersonalInfoRequestViewModel personalInfo);
        Task<ListUsersViewModel<BaseUserViewModel>> GetListUsers(BaseSearchRequest args);
        Task<ListUsersViewModel<EmployeeViewModel>> GetListEmployees(BaseSearchRequest args);
        Task<EmployeeViewModel>? GetEmployee(Guid id);
        List<IdentityRole> GetRoles();
        Task<bool> CreateEmployee(ShopUser user, string userName, string userPWD, List<string> roles);
        Task<bool> UpdateEmployee(EmployeeViewModel userModel);

    }
}

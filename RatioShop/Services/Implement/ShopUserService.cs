using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.User;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Services.Implement
{
    public class ShopUserService : IShopUserService
    {
        private readonly IShopUserRepository _shopUserRepository;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly UserManager<ShopUser> _userManager;
        private readonly IUserStore<ShopUser> _userStore;
        private readonly IAddressService _addressService;
        private readonly IUserEmailStore<ShopUser> _emailStore;

        public ShopUserService(IShopUserRepository shopUserRepository, SignInManager<ShopUser> signInManager, UserManager<ShopUser> userManager, IUserStore<ShopUser> userStore, IAddressService addressService)
        {
            _shopUserRepository = shopUserRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _addressService = addressService;
            _emailStore = GetEmailStore();
        }


        #region Basic action
        public Task<ShopUser> CreateShopUser(ShopUser ShopUser)
        {
            return _shopUserRepository.CreateShopUser(ShopUser);
        }

        public bool DeleteShopUser(string id)
        {
            return _shopUserRepository.DeleteShopUser(id);
        }

        public IEnumerable<ShopUser> GetShopUsers()
        {
            return _shopUserRepository.GetShopUsers();
        }

        public ShopUser? GetShopUser(string id)
        {
            return _shopUserRepository.GetShopUser(id);
        }

        public bool UpdateShopUser(ShopUser ShopUser)
        {
            return _shopUserRepository.UpdateShopUser(ShopUser);
        }
        #endregion

        private async Task<bool> UpdatePhoneNumber(ShopUser user, string newPhoneNumber)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (newPhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }
        private IUserEmailStore<ShopUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ShopUser>)_userStore;
        }
        public async Task<RegisterResponseViewModel> RegisterUser(RegisterRequestViewModel newUser)
        {
            if (newUser == null || (!newUser.IsExternalLogin && (string.IsNullOrWhiteSpace(newUser.UserName) || (string.IsNullOrEmpty(newUser.Password))))) return new RegisterResponseViewModel { Status = "Failure" };

            var user = new ShopUser
            {
                AddressDetail = newUser.AddressDetail,
                AddressId = _addressService.GetAddressByCityAndDistrict(newUser.City, newUser.District)?.Id,
                FullName = newUser.FullName
            };

            await _userStore.SetUserNameAsync(user, newUser.UserName, CancellationToken.None);
            //await _emailStore.SetEmailAsync(user, newUser.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, newUser.Password);

            if (result.Succeeded)
            {
                await UpdatePhoneNumber(user, newUser.PhoneNumber);
                await _userManager.AddToRoleAsync(user, CommonConstant.Customer);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new RegisterResponseViewModel
                {
                    UserName = user.UserName,
                    Status = "Success"
                };
            }

            return new RegisterResponseViewModel { Status = "Failure" };
        }

        public async Task<LoginResponseViewModel> UserLogin(LoginRequestViewModel request)
        {
            if (request == null || (!request.IsExternalLogin && (string.IsNullOrWhiteSpace(request.UserName) || (string.IsNullOrEmpty(request.Password))))) return new LoginResponseViewModel { Status = "Failure" };

            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)) return new LoginResponseViewModel { Status = "Failure" };
                                         
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return new LoginResponseViewModel
                {
                    UserName = request.UserName,
                    Status = "Success"
                };
            }

            return new LoginResponseViewModel { Status = "Failure" };
        }

        public async Task<LoginPopupViewModel> LoginOrRegisterPopupAction(bool isLoginPopup)
        {
            var result = new LoginPopupViewModel
            {
                StoreName = CommonConstant.StoreName,
                Logo = "/images/icons/logo-01.png",
                Description = $"By continuing, you agree to {CommonConstant.StoreName}'s Conditions of Use and Privacy Notice."
            };

            if (isLoginPopup)
            {
                // Clear the existing external cookie to ensure a clean login process
                await _signInManager.SignOutAsync();
            }

            result.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Select(x => x.Name).ToList();
            result.ListCities = _addressService.GetAddressesByType("Address1").ToList();
        
            return result;
        }

        public async Task<bool> UserLogout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ChallengeResult ExternalLogin(string provider, string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<LoginResponseViewModel> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new LoginResponseViewModel { Status = "Failure" };
            }

            // check can login yet?
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return new LoginResponseViewModel { Status = "Success", UserName = info.Principal.Identity.Name };
            }

            if (signInResult.IsLockedOut)
            {
                return new LoginResponseViewModel { Status = "Failure", Message = "Account being locked!" };
            }
            else
            {
                //                                
                var user = new ShopUser()
                {
                    AddressDetail = info.Principal.FindFirstValue(ClaimTypes.StreetAddress)
                };
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                string phoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await UpdatePhoneNumber(user, phoneNumber);
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return new LoginResponseViewModel { Status = "Success", UserName = user.UserName };
                    }
                }
            }

            return new LoginResponseViewModel { Status = "Failure", Message = $"External authentication by {info.LoginProvider} failure!" };
        }

        public bool UpdateShopUserNewShippingAddressInfor(ShopUser ShopUser, string fullName, string phoneNumber, string adderssDetail, int? addressId)
        {
            var isChange = false;
            if (string.IsNullOrWhiteSpace(ShopUser.FullName) && !string.IsNullOrWhiteSpace(fullName))
            {
                ShopUser.FullName = fullName;
                isChange = true;
            }
            if (string.IsNullOrWhiteSpace(ShopUser.PhoneNumber) && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                ShopUser.PhoneNumber = phoneNumber;
                isChange = true;
            }
            if (string.IsNullOrWhiteSpace(ShopUser.AddressDetail) && !string.IsNullOrWhiteSpace(adderssDetail))
            {
                ShopUser.AddressDetail = adderssDetail;
                isChange = true;
            }
            if ((ShopUser.AddressId == null || ShopUser.AddressId == 0) && (addressId != null && addressId != 0))
            {
                ShopUser.AddressId = addressId;
                isChange = true;
            }
            if (isChange) return UpdateShopUser(ShopUser);

            return true;
        }

        public bool UpdateShopUserNewShippingAddressInfor(ShopUser? shopUser, Cart? cart)
        {
            if (cart == null || shopUser == null) return false;
            return UpdateShopUserNewShippingAddressInfor(shopUser, cart.FullName, cart.PhoneNumber, cart.AddressDetail, cart.AddressId);
        }

        public UserViewModel GetShopUserViewModel(string id)
        {
            var result = new UserViewModel();
            var user = GetShopUser(id);
            if (user == null) return result;

            if (user.AddressId != null && user.AddressId != 0)
            {
                var userAddress = _addressService.GetAddress((int)user.AddressId);
                user.Address = userAddress;
                result.DefaultShippingAddress = $"{user.AddressDetail} - {userAddress?.Address2} - {userAddress?.Address1}";
            }
            result.User = user;

            return result;
        }

        public bool UpdatePersonalInformation(string id, PersonalInfoRequestViewModel personalInfo)
        {
            if (string.IsNullOrEmpty(id) || personalInfo == null) return false;

            var user = GetShopUser(id);
            if(user == null) return false;

            var addressId = _addressService.GetAddressByCityAndDistrict(personalInfo.City, personalInfo.District)?.Id;
            if(addressId == null || addressId == 0) return false;

            user.PhoneNumber = personalInfo.PhoneNumber;
            user.AddressDetail = personalInfo.AddressDetail;
            user.FullName = personalInfo.FullName;
            user.AddressId = addressId;

            return UpdateShopUser(user);
        }
    }
}

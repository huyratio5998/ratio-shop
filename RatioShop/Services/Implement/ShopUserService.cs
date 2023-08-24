using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models.User;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Data.ViewModels.User;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public ShopUserService(IShopUserRepository shopUserRepository, SignInManager<ShopUser> signInManager, UserManager<ShopUser> userManager, IUserStore<ShopUser> userStore, IAddressService addressService, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _shopUserRepository = shopUserRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _addressService = addressService;
            _emailStore = GetEmailStore();
            _roleManager = roleManager;
            _mapper = mapper;
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
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

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
                    AddressDetail = info.Principal.FindFirstValue(ClaimTypes.StreetAddress),
                    FullName = info.Principal.FindFirstValue(ClaimTypes.Name)
                };
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                string phoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                // check username exist
                if(_shopUserRepository.GetShopUsers().FirstOrDefault(x=>x.UserName.Equals(email)) != null)
                {
                    email = $"{email}.{info.LoginProvider}".ToLower();
                }

                await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await UpdatePhoneNumber(user, phoneNumber);
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
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
            if (user == null) return false;

            var addressId = _addressService.GetAddressByCityAndDistrict(personalInfo.City, personalInfo.District)?.Id;
            if (addressId == null || addressId == 0) return false;

            user.PhoneNumber = personalInfo.PhoneNumber;
            user.AddressDetail = personalInfo.AddressDetail;
            user.FullName = personalInfo.FullName;
            user.AddressId = addressId;

            return UpdateShopUser(user);
        }

        public async Task<ListUsersViewModel<BaseUserViewModel>> GetListUsers(BaseSearchRequest args)
        {
            int totalCount = 0;
            var users = GetShopUsers(args, out totalCount);            

            var usersInfo = _mapper.Map<List<BaseUserViewModel>>(users);

            if (usersInfo != null && usersInfo.Any())
            {
                foreach (var item in usersInfo)
                {
                    var user = GetShopUser(item.Id.ToString());
                    if (user == null) continue;

                    var roles = await GetUserRoles(user);
                    item.UserRole = String.Join(", ", roles);
                }
            }

            totalCount = usersInfo.Count;
            args.PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex;
            usersInfo = usersInfo.OrderBy(x => x.UserRole).ThenBy(x => x.FullName).Skip((args.PageIndex - 1) * args.PageSize).Take(args.PageSize).ToList();

            var result = new ListUsersViewModel<BaseUserViewModel>
            {
                Users = usersInfo,
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
            return result;
        }

        public async Task<ListUsersViewModel<EmployeeViewModel>> GetListEmployees(BaseSearchRequest args)
        {
            int totalCount = 0;
            var users = GetShopUsers(args, out totalCount)?.ToList();

            Dictionary<string, string> userRoles = new Dictionary<string, string>();
            if (users != null && users.Any())
            {
                foreach (var item in users)
                {
                    var roles = await GetUserRoles(item);
                    if (roles == null) continue;

                    if (roles.Contains(UserRole.Employee.ToString())
                        || roles.Contains(UserRole.Admin.ToString())
                        || roles.Contains(UserRole.Manager.ToString())
                        || roles.Contains(UserRole.SuperAdmin.ToString())
                        || roles.Contains(UserRole.ContentEditor.ToString())
                        )
                    {
                        userRoles.Add(item.Id.ToString(), string.Join(", ", roles));
                    }
                }
            }
            var usersInfo = _mapper.Map<List<EmployeeViewModel>>(users);
            List<EmployeeViewModel> employeesInfo = new List<EmployeeViewModel>();
            foreach (var item in userRoles)
            {
                var employee = usersInfo.FirstOrDefault(x => x.Id.ToString().Equals(item.Key));
                if (employee == null) continue;

                employee.UserRole = item.Value;
                employeesInfo.Add(employee);
            }

            totalCount = employeesInfo.Count;
            args.PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex;
            employeesInfo = employeesInfo.OrderBy(x => x.UserRole).ThenBy(x=>x.EmployeeCode).Skip((args.PageIndex - 1) * args.PageSize).Take(args.PageSize).ToList();

            var result = new ListUsersViewModel<EmployeeViewModel>
            {
                Users = employeesInfo,
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
            return result;
        }

        public async Task<List<string>>? GetUserRoles(ShopUser user)
        {
            if (user == null) return null;

            var result = await _userManager.GetRolesAsync(user);
            return result.ToList();
        }

        private IQueryable<ShopUser>? GetShopUsers(BaseSearchRequest args, out int totalCount)
        {
            var users = GetShopUsers().AsQueryable();
            users = BuildShopUserFilters(users, args);
            users = users?.OrderBy(x => x.FullName);

            totalCount = users?.Count() ?? 0;

            return users;
        }
        private IQueryable<ShopUser>? BuildShopUserFilters(IQueryable<ShopUser>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            var predicate = PredicateBuilder.True<ShopUser>();
            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.Text:
                        {
                            switch (item.FieldName)
                            {
                                case "":
                                    break;
                                case "EmployeeCode":
                                    predicate = predicate.And(x => x.EmployeeCode.Contains(item.Value));
                                    break;
                                case "EmployeeName":
                                    predicate = predicate.And(x => x.EmployeeName.Contains(item.Value));
                                    break;
                                case "FullName":
                                    predicate = predicate.And(x => x.FullName.Contains(item.Value));
                                    break;
                                case "PhoneNumber":
                                    predicate = predicate.And(x => x.PhoneNumber.Contains(item.Value));
                                    break;
                                case "Email":
                                    predicate = predicate.And(x => x.Email.Contains(item.Value));
                                    break;
                                case "City":
                                    {
                                        predicate = predicate.And(x => x.AddressId != null && _addressService.GetAddress((int)x.AddressId) != null && _addressService.GetAddress((int)x.AddressId).Address1.Equals(item.Value));
                                        break;
                                    }
                                case "District":
                                    predicate = predicate.And(x => x.AddressId != null && _addressService.GetAddress((int)x.AddressId) != null && _addressService.GetAddress((int)x.AddressId).Address2.Equals(item.Value));
                                    break;
                            }
                            break;
                        }
                }
            }

            return queries.Where(predicate);
        }

        public async Task<EmployeeViewModel>? GetEmployee(Guid id)
        {
            if (id == Guid.Empty) return null;

            var user = GetShopUser(id.ToString());
            if(user == null) return null;

            // address
            if (user.AddressId != null && user.AddressId != 0)
            {
                var userAddress = _addressService.GetAddress((int)user.AddressId);
                user.Address = userAddress;                
            }

            var result = _mapper.Map<EmployeeViewModel>(user);
            result.UserRoles = await GetUserRoles(user);
            result.UserRole = String.Join(", ", result.UserRoles);

            return result;
        }

        public List<IdentityRole> GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return roles;
        }

        public async Task<bool> CreateEmployee(ShopUser user, string userName, string userPWD, List<string> roles)
        {           
            try
            {
                await _userStore.SetUserNameAsync(user, userName, CancellationToken.None);
                var createPowerUser = await _userManager.CreateAsync(user, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await UpdatePhoneNumber(user, user.PhoneNumber);
                    await _userManager.AddToRolesAsync(user, roles);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployee(EmployeeViewModel userModel)
        {            
            var user = await _userManager.FindByIdAsync(userModel.Id.ToString());

            if (user == null) return false;

            // update basic info
            var needUpdateBasic = false;
            if (!userModel.EmployeeCode.Equals(user.EmployeeCode))
            {
                var exitUser = GetShopUsers().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.EmployeeCode) && x.EmployeeCode.Equals(userModel.EmployeeCode));
                if (exitUser != null && exitUser.Id != Guid.Empty.ToString()) return false;

                user.EmployeeCode = userModel.EmployeeCode;
                needUpdateBasic = true;
            }
            if (!userModel.EmployeeName.Equals(user.EmployeeName))
            {
                user.EmployeeName = userModel.EmployeeName;
                needUpdateBasic = true;
            }

            var updateBasicInfo = needUpdateBasic ? UpdateShopUser(user) : true;

            // update role
            var currentUserRoles = await _userManager.GetRolesAsync(user);            
            var isEqual = currentUserRoles?.OrderBy(x => x).ToList().SequenceEqual(userModel.UserRoles.OrderBy(x => x));
            bool updateRoleStatus = true;
            if (isEqual != true)
            {                                
                await _userManager.RemoveFromRolesAsync(user, currentUserRoles);

                var result = await _userManager.AddToRolesAsync(user, userModel.UserRoles);
                updateRoleStatus = result.Succeeded;
            }

            // update phone number
            bool setPhoneStatus = true;
            if (userModel.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userModel.PhoneNumber);
                setPhoneStatus = setPhoneResult.Succeeded;
            }            

            return updateBasicInfo && updateRoleStatus && setPhoneStatus;
        }        
    }
}

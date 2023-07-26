using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models.User;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/userManagements")]
    [Authorize(Roles = "Admin,Manager,SuperAdmin")]
    public class UserManagementsController : Controller
    {
        private readonly IShopUserService _userService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;
        public UserManagementsController(IShopUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterOrder(string actionRedirect, string? employeeCode, string? employeeName, string? fullName, string? phoneNumber, string? email, string? city, string? district, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            if (!string.IsNullOrWhiteSpace(employeeCode))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "EmployeeCode", Type = FilterType.Text.ToString(), Value = employeeCode });
            if (!string.IsNullOrWhiteSpace(employeeName))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "EmployeeName", Type = FilterType.Text.ToString(), Value = employeeName });
            if (!string.IsNullOrWhiteSpace(fullName))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "FullName", Type = FilterType.Text.ToString(), Value = fullName });
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "PhoneNumber", Type = FilterType.Text.ToString(), Value = phoneNumber });
            if (!string.IsNullOrWhiteSpace(email))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Email", Type = FilterType.Text.ToString(), Value = email });
            if (!string.IsNullOrWhiteSpace(city))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "City", Type = FilterType.Text.ToString(), Value = city });
            if (!string.IsNullOrWhiteSpace(district))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "District", Type = FilterType.Text.ToString(), Value = district });

            if (page <= 1) page = null;

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), page = page });
        }

        public async Task<IActionResult> Index(BaseSearchArgs args)
        {
            var request = _mapper.Map<BaseSearchRequest>(args);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var users = await _userService.GetListUsers(request);

            if (users == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "UserManagements";
            ViewBag.Action = "Index";
            ViewBag.DetailParam = "Index";

            return View(users);
        }

        [Route("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (id == Guid.Empty) return RedirectToAction("Index", "Home");

            var userDetail = await _userService.GetEmployee(id);

            if (userDetail == null) return RedirectToAction("Index", "Home");

            return View(userDetail);
        }

        [Route("employees")]
        public async Task<IActionResult> Employees(BaseSearchArgs args)
        {
            var request = _mapper.Map<BaseSearchRequest>(args);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var users = await _userService.GetListEmployees(request);
            if (users == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "UserManagements";
            ViewBag.Action = "Employees";
            ViewBag.DetailParam = "Employees";
            return View(users);
        }

        [HttpGet("employeeDetail")]
        public async Task<IActionResult> EmployeeDetail(Guid id)
        {
            if (id == Guid.Empty) return RedirectToAction("Index", "Home");

            var employeeDetail = await _userService.GetEmployee(id);

            if (employeeDetail == null) return RedirectToAction("Index", "Home");

            return View(employeeDetail);
        }

        [HttpGet("createEmployee")]
        public IActionResult CreateEmployee()
        {
            var availableRoles = _userService.GetRoles().Select(x => x.Name)?.ToList();

            var result = new EmployeeViewModel()
            {
                AvailableRoles = availableRoles,
            };

            return View(result);
        }

        [HttpPost("createEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel employee)
        {
                var availableRoles = _userService.GetRoles().Select(x => x.Name)?.ToList();
                employee.AvailableRoles = availableRoles;
            if (employee == null || !ModelState.IsValid || string.IsNullOrWhiteSpace(employee.Username) || string.IsNullOrWhiteSpace(employee.Password))
            {
                ViewBag.ErrorMessage = "Bad request!";
                return View(employee);
            }
            var exitUser = _userService.GetShopUsers().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.EmployeeCode) && x.EmployeeCode.Equals(employee.EmployeeCode));
            if (exitUser != null && exitUser.Id != Guid.Empty.ToString())
            {
                ViewBag.ErrorMessage = "Employee code exist!";
                return View(employee);
            }

            var user = _mapper.Map<ShopUser>(employee);

            if (string.IsNullOrEmpty(user.Id) || user.Id.Equals(Guid.Empty.ToString())) user.Id = Guid.NewGuid().ToString();

            if (!employee.UserRoles.Any()) employee.UserRoles.Add(UserRole.Employee.ToString());                       
            var result = await _userService.CreateEmployee(user, employee.Username, employee.Password, employee.UserRoles);

            if (!result)
            {
                ViewBag.ErrorMessage = "Fail to create employee!";
                return View(employee);
            }
            return RedirectToAction("Employees");
        }

        [HttpGet("updateEmployee")]
        public async Task<IActionResult> UpdateEmployee(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var employeeDetail = await _userService.GetEmployee(userId);            
            
            employeeDetail.AvailableRoles = _userService.GetRoles().Select(x => x.Name)?.ToList();

            if (employeeDetail == null) return RedirectToAction("Employees");

            return View(employeeDetail);
        }

        [HttpPost("updateEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel employee)
        {
                var availableRoles = _userService.GetRoles().Select(x => x.Name)?.ToList();
                employee.AvailableRoles = availableRoles;
            if (employee == null || !ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Bad Request!";
                return View(employee);
            }

            var result = await _userService.UpdateEmployee(employee);

            if (!result)
            {
                ViewBag.ErrorMessage = "Fail to update employee info!";
                return View(employee);
            }

            return RedirectToAction("Employees");
        }

        [HttpGet("deleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var employeeDetail = await _userService.GetEmployee(userId);

            if (employeeDetail == null) return RedirectToAction("Employees");

            return View(employeeDetail);
        }

        [HttpPost("deleteEmployeeConfirmed")]        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployeeConfirmed(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var deleteStatus = _userService.DeleteShopUser(userId.ToString());

            if (!deleteStatus)
            {
                var employeeDetail = await _userService.GetEmployee(userId);
                return View(employeeDetail);
            }

            return RedirectToAction("Employees");
        }
    }

}

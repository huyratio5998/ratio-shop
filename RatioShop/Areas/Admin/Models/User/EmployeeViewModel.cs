namespace RatioShop.Areas.Admin.Models.User
{
    public class EmployeeViewModel : BaseUserViewModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}

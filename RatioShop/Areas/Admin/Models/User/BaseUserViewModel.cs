namespace RatioShop.Areas.Admin.Models.User
{
    public class BaseUserViewModel
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressDetail { get; set; }
        public string? Email { get; set; }

        public int? AddressId { get; set; }
        public string? ShippingAddress1 { get; set; }
        public string? ShippingAddress2 { get; set; }
        public string? FullShippingAddress { get; set; }
        
        public List<string> UserRoles { get; set; } = new List<string>();
        public string? UserRole { get; set; }

        public List<string>? AvailableRoles { get; set; }
    }
}

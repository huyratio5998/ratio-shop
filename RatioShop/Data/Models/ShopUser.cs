using Microsoft.AspNetCore.Identity;

namespace RatioShop.Data.Models
{
    public class ShopUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? AddressDetail { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public List<Cart>? Carts { get; set; }
        public List<Shipment>? Shipments { get; set; }
    }
}

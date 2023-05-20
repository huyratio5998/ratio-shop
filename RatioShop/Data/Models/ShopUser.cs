using Microsoft.AspNetCore.Identity;

namespace RatioShop.Data.Models
{
    public class ShopUser : IdentityUser
    {
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public List<Cart>? Carts { get; set; }
    }
}

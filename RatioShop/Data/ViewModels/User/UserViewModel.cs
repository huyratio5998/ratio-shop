using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels.User
{
    public class UserViewModel
    {
        public ShopUser? User { get; set; }        
        public string DefaultShippingAddress { get; set; }
    }
}

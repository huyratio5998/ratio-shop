using RatioShop.Data.ViewModels.Cart;

namespace RatioShop.Data.ViewModels
{
    public class CartDetailViewModel
    {        
        public List<CartItemResponseViewModel>? CartItems { get; set; }        
        public int TotalItems { get; set; }        
        public decimal TotalPrice { get; set; }
        public decimal TotalFinalPrice { get; set; }
        public string? Status { get; set; }

        public List<string>? CouponCodes { get; set; }
        public AddressResponseViewModel? ShippingAddress { get; set; }        
        public DefaultShippingAddressViewModel? ShippingAddressDefault { get; set; }
        public string? FullShippingAddress { get; set; }
        public bool IsEnoughShippingInformation { get; set; }
        public decimal? ShippingFee { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public Guid UserID { get; set; }
        public List<string> ListCities { get; set; }
        public List<string>? ListDistrict { get; set; }
    }
}

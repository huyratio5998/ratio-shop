using Newtonsoft.Json;
using RatioShop.Data.ViewModels.Cart;

namespace RatioShop.Data.ViewModels
{
    public class CartDetailResponsetViewModel
    {
        [JsonProperty("cartItems")]
        public List<CartItemResponseViewModel> CartItems { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }

        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonProperty("totalFinalPrice")]
        public decimal TotalFinalPrice { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("couponCodes")]
        public List<string>? CouponCodes { get; set; }

        // Default shipping address

        [JsonProperty("shippingAddressDefault")]
        public DefaultShippingAddressViewModel? ShippingAddressDefault { get; set; }

        [JsonProperty("isEnoughShippingInformation")]
        public bool IsEnoughShippingInformation { get; set; }

        // Shipping Address
        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("shippingAddressId")]       
        public int? ShippingAddressId { get; set; }

        [JsonProperty("fullShippingAddress")]
        public string? FullShippingAddress { get; set; }

        [JsonProperty("shippingAddressDetail")]
        public string? ShippingAddressDetail { get; set; }  
        
        [JsonProperty("shippingFee")]
        public decimal? ShippingFee { get; set; }       
      
        // User
        [JsonProperty("userID")]
        public Guid UserID { get; set; }
    }
}

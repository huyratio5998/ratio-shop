using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.Cart
{
    public class DefaultShippingAddressViewModel
    {
        // Shipping Address
        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("shippingAddressId")]
        public int? ShippingAddressId { get; set; }

        [JsonProperty("shippingAddressDetail")]
        public string? ShippingAddressDetail { get; set; }        
    }
}

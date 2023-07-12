using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels
{
    public class CheckoutResponseViewModel
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

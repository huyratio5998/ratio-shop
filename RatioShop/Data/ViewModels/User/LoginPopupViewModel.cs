using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.User
{
    public class LoginPopupViewModel
    {
        [JsonProperty("storeName")]
        public string StoreName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("externalLogins")]
        public List<string>? ExternalLogins { get; set; }

        [JsonProperty("listCities")]
        public List<string>? ListCities { get; set; }
    }
}

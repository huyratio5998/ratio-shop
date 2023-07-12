using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels
{
    public class BaseAuthenticateViewModel
    {
        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; } = true;
        [JsonProperty("isAuthorized")]
        public bool IsAuthorized { get; set; } = true;
    }
}

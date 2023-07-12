using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.User
{
    public class ExternalLoginRequestViewModel
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }
    }
}

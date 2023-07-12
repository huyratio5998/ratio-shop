using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.User
{
    public class LoginResponseViewModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.User
{
    public class RegisterResponseViewModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }        
    }
}

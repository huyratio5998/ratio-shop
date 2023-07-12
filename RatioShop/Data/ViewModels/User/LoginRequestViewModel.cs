using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.ViewModels.User
{
    public class LoginRequestViewModel
    {
        [JsonProperty("userName")]
        [RegularExpression("^[a-zA-Z0-9]+$",ErrorMessage = "Special characters are not allowed")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("rememberMe")]
        public bool RememberMe { get; set; }
        [JsonProperty("isexternallogin")]
        public bool IsExternalLogin { get; set; }
    }
}

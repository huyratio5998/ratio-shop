using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.ViewModels.User
{
    public class RegisterRequestViewModel
    {
        [JsonProperty("userName")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Special characters are not allowed")]
        public string? UserName { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("district")]
        public string? District { get; set; }

        [JsonProperty("addressDetail")]
        public string? AddressDetail { get; set; }
       
        [JsonProperty("isExternalLogin")]
        public bool IsExternalLogin { get; set; }
    }
}

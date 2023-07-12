using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.User
{
    public class PersonalInfoRequestViewModel
    {        
        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("district")]
        public string? District { get; set; }

        [JsonProperty("addressDetail")]
        public string? AddressDetail { get; set; }        
    }
}

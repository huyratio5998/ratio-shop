using Newtonsoft.Json;
using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels.User
{
    public class UserResponseViewModel
    {        
        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }        

        [JsonProperty("address")]
        public AddressResponseViewModel? Address { get; set; }        
    }
}

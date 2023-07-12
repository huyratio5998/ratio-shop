using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels
{
    public class AddToCartResponsetViewModel : BaseAuthenticateViewModel
    {
        public AddToCartResponsetViewModel()
        {
        }

        public AddToCartResponsetViewModel(Guid cartId, string status, string message)
        {
            CartId = cartId;
            Status = status;
            Message = message;
        }
        public AddToCartResponsetViewModel(Guid cartId, string status, string message, bool isAuthenticated, bool isAuthorized)
        {
            CartId = cartId;
            Status = status;
            Message = message;
            IsAuthenticated = isAuthenticated;
            IsAuthorized = isAuthorized;
        }

        [JsonProperty("cartId")]
        public Guid CartId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }       

    }
}

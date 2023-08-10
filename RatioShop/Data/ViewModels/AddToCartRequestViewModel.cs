using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels
{
    public class AddToCartRequestViewModel
    {
        [JsonProperty("cartId")]
        public Guid? CartId { get; set; }

        [JsonProperty("userId")]
        public Guid? UserId { get; set; }

        [JsonProperty("variantId")]
        public Guid? VariantId { get; set; }

        [JsonProperty("packageId")]
        public Guid? PackageId { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }       
    }
}

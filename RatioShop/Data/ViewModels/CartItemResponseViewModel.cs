using Newtonsoft.Json;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Enums;

namespace RatioShop.Data.ViewModels
{
    public class CartItemResponseViewModel
    {
        [JsonProperty("variantId")]
        public Guid VariantId { get; set; }

        [JsonProperty("packageId")]
        public Guid? PackageId { get; set; }

        [JsonProperty("itemType")]
        public CartItemType ItemType { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("packageNumber")]
        public int PackageNumber { get; set; }

        [JsonProperty("enableStockTracking")]
        public bool EnableStockTracking { get; set; }

        [JsonProperty("stockItems")]
        public List<CartStockItem>? StockItems { get; set; }

        [JsonProperty("variableName")]
        public string? VariableName { get; set; }
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("discountPrice")]
        public decimal? DiscountPrice { get; set; }

        [JsonProperty("discountRate")]
        public double? DiscountRate { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("productCode")]
        public string? ProductCode { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

    }
}

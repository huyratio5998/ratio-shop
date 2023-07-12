using Newtonsoft.Json;
using RatioShop.Data.ViewModels.Cart;

namespace RatioShop.Data.ViewModels
{
    public class CartItemResponseViewModel
    {
        [JsonProperty("variantId")]
        public Guid VariantId { get; set; }        
        [JsonProperty("number")]
        public int Number { get; set; }
        
        [JsonProperty("enableStockTracking")]
        public bool EnableStockTracking { get; set; }

        [JsonProperty("stockItems")]
        public List<CartStockItem>? StockItems { get; set; }

        [JsonProperty("variableName")]
        public string? VariableName { get; set; }
        [JsonProperty("price")]
        public decimal? Price { get; set; }  
        
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("image")]
        public string? Image { get; set; }

    }
}

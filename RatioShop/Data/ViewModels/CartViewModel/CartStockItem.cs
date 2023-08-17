using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.CartViewModel
{
    public class CartStockItem
    {
        [JsonProperty("stockId")]
        public int StockId { get; set; }

        [JsonProperty("itemNumber")]
        public int ItemNumber { get; set; }
    }
}

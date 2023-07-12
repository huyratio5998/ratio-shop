﻿using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.Cart
{
    public class CartStockItem
    {
        [JsonProperty("stockId")]
        public int StockId { get; set; }

        [JsonProperty("itemNumber")]
        public int ItemNumber { get; set; }
    }
}

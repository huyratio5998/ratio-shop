using Newtonsoft.Json;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Data.ViewModels.ShipmentViewModel;

namespace RatioShop.Data.ViewModels.OrdersViewModel
{
    public class OrderResponseViewModel
    {
        public OrderResponseViewModel()
        {

        }
        [JsonProperty("orderId")]
        public Guid? OrderId { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }
        
        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("orderNumber")]
        public string? OrderNumber { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("totalMoney")]
        public decimal? TotalMoney { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }

        [JsonProperty("shipmentStatus")]
        public string? ShipmentStatus { get; set; }

        [JsonProperty("shipmentFee")]
        public decimal? ShipmentFee { get; set; }

        [JsonProperty("isRefund")]
        public bool IsRefund { get; set; }

        [JsonProperty("cartDetail")]
        public CartDetailResponsViewModel? CartDetail { get; set; }

        [JsonProperty("shipmentHistory")]
        public ShipmentsResponseViewModel? ShipmentHistory { get; set; }

        [JsonProperty("payment")]
        public PaymentResponseViewModel? Payment { get; set; }
        
    }
}

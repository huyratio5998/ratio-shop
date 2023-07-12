namespace RatioShop.Data.Models
{
    public class Order : BaseProduct
    {
        public string OrderNumber { get; set; }
        public string? Status { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool IsRefund { get; set; }
        public string? ShipmentStatus { get; set; }
        public decimal? ShipmentFee { get; set; }

        public Guid CartId { get; set; }
        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public List<Shipment>? Shipments { get; set; }
    }
}
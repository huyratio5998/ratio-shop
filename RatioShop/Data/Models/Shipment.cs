namespace RatioShop.Data.Models
{
    public class Shipment : BaseProduct
    {
        public string? Request { get; set; }        
        public bool? UpdateStatus { get; set; }
        public string ShipmentStatus { get; set; }
        public string? Reasons { get; set; }
        public string? Images { get; set; }
        public string? ShipperId { get; set; }
        public ShopUser? Shipper { get; set; }

        public string? SystemMessage { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}

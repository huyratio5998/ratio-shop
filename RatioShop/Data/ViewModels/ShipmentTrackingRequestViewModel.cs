namespace RatioShop.Data.ViewModels
{
    public class ShipmentTrackingRequestViewModel
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public string? Reasons { get; set; }
        public string? Images { get; set; }
        public Guid ShipperId { get; set; }
    }
}

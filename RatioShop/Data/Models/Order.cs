namespace RatioShop.Data.Models
{
    public class Order : BaseProduct
    {
        public string? Status { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool IsRefund { get; set; }
        public string? ShipmentStatus { get; set; }
        public decimal? ShipmentFee { get; set; }

        public Guid CartId { get; set; }
        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}
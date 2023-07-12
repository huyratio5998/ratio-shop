namespace RatioShop.Data.Models
{
    public class ProductVariantCart : BaseProduct
    {
        public string? StockItems { get; set; }
        public bool TrackUpdated { get; set; }
        public int ItemNumber { get; set; }
        public string? StockTrackingStatus { get; set; }
        public bool IsReverted { get; set; }
        public Guid ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }
    }
}
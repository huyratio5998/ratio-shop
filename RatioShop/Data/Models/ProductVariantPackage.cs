namespace RatioShop.Data.Models
{
    public class ProductVariantPackage
    {
        public int ItemNumber { get; set; }
        public Guid ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
    }
}
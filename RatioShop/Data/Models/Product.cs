namespace RatioShop.Data.Models
{
    public class Product : BaseProduct
    {
        public string? Code { get; set; }
        public string? Name { get; set; }        
        public string? ProductFriendlyName { get; set; }
        public string? ProductRawName { get; set; }
        public string? Description { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }
        public bool EnableStockTracking { get; set; }

        // Media
        public string? ProductImage { get; set; }
        // Techspec

        public List<ProductVariant>? Variants { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }        
    }
}

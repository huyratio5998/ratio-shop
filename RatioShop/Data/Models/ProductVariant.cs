namespace RatioShop.Data.Models
{
    public class ProductVariant : BaseProduct
    {
        public string? Code { get; set; }        
        public int? Number { get; set; }
        public decimal? Price { get; set; }
        public double? DiscountRate { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public List<ProductVariantStock>? productVariantStocks { get; set; }
    }
}
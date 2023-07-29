using RatioShop.Enums;

namespace RatioShop.Data.ViewModels
{
    public class ProductVariantRequestViewModel
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public int? Number { get; set; }
        public decimal? Price { get; set; }
        public double? DiscountRate { get; set; }
        public string? Images { get; set; }
        public VariantType Type { get; set; }
        public List<ProductVariantStockViewModel> ProductVariantStocks { get; set; }
    }
}

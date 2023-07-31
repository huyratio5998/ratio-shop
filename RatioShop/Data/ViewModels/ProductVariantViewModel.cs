namespace RatioShop.Data.ViewModels
{
    public class ProductVariantViewModel
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Code { get; set; }
        public int? Number { get; set; }
        public decimal? PriceAfterDiscount { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
    }
}
namespace RatioShop.Data.Models
{
    public class Cart : BaseProduct
    {
        public string? Status { get; set; }

        public Guid ProductVariantCartId { get; set; }
        public ProductVariantCart? ProductVariantCart { get; set; }
        public string ShopUserId { get; set; }
        public ShopUser? ShopUser { get; set; }        
    }
}
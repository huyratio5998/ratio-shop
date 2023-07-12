namespace RatioShop.Data.Models
{
    public class Cart : BaseProduct
    {
        public string? Status { get; set; }
        public int? AddressId { get; set; }
        public string? AddressDetail { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string ShopUserId { get; set; }
        public ShopUser? ShopUser { get; set; }
        public List<ProductVariantCart>? ProductVariantCarts { get; set; }
        public List<CartDiscount>? CartDiscounts { get; set; }
    }
}
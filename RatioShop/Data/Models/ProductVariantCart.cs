namespace RatioShop.Data.Models
{
    public class ProductVariantCart : BaseProduct
    {
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public List<Cart> Carts { get; set; }
    }
}
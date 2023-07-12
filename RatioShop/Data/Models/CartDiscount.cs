namespace RatioShop.Data.Models
{
    public class CartDiscount : BaseEntity
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public int DiscountId { get; set; }
        public Discount Discount { get; set; }        
    }
}
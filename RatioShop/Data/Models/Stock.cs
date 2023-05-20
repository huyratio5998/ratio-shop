namespace RatioShop.Data.Models
{
    public class Stock : BaseEntity
    {
        public string? Name { get; set; }
        public bool IsActive { get; set; }        

        public List<ProductVariantStock>? ProductVariantStocks { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.Models
{
    public class Address : BaseEntity
    {
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? Address5 { get; set; }
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode =true)]
        public decimal ShippingFee { get; set; }
        public bool IsActive { get; set; }

        public List<Stock> Stocks { get; set; }
        public List<ShopUser> Users { get; set; }

    }
}
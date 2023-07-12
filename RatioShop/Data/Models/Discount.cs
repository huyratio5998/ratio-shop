using RatioShop.Enums;
using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.Models
{
    public class Discount : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Number { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Value { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Status { get; set; }

        public List<CartDiscount>? CartDiscounts { get; set; }
    }
}

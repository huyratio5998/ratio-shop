using RatioShop.Enums;

namespace RatioShop.Data.Models
{
    public class Payment: BaseProduct
    {
        public string? Logo { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public PaymentType Type { get; set; }

        public List<Order>? Orders { get; set; }
    }
}
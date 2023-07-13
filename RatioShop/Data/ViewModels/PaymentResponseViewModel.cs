using RatioShop.Enums;

namespace RatioShop.Data.ViewModels
{
    public class PaymentResponseViewModel
    {
        public Guid Id { get; set; }
        public string? Logo { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public PaymentType Type { get; set; }
    }
}
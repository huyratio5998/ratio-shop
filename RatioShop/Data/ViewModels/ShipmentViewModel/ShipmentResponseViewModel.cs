using RatioShop.Data.ViewModels.User;

namespace RatioShop.Data.ViewModels.ShipmentViewModel
{
    public class ShipmentResponseViewModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? UpdateStatus { get; set; }
        public string ShipmentStatus { get; set; }
        public string? Reasons { get; set; }
        public string? Images { get; set; }
        public UserResponseViewModel? Shipper { get; set; }
    }
}

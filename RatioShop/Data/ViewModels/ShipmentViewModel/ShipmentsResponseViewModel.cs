using RatioShop.Data.ViewModels.User;

namespace RatioShop.Data.ViewModels.ShipmentViewModel
{
    public class ShipmentsResponseViewModel
    {
        public List<ShipmentResponseViewModel> Shipments { get; set; }
        public List<UserResponseViewModel>? AvailableShippers { get; set; }
    }
}

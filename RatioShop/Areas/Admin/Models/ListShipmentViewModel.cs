using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.OrdersViewModel;

namespace RatioShop.Areas.Admin.Models
{
    public class ListShipmentViewModel : BaseListingPageViewModel
    {
        public ListShipmentViewModel()
        {
            if (Orders == null) Orders = new List<OrderResponseViewModel>();
        }
        public List<OrderResponseViewModel> Orders { get; set; }
    }
}

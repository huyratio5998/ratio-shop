namespace RatioShop.Data.ViewModels.MyAccountViewModel
{
    public class ListOrderViewModel : BaseListingPageViewModel
    {
        public ListOrderViewModel()
        {
            if (Orders == null) Orders = new List<OrderViewModel>();
        }
        public IEnumerable<OrderViewModel> Orders { get; set; }
    }
}

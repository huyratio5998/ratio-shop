namespace RatioShop.Data.ViewModels.OrdersViewModel
{
    public class OrdersResponseViewModel : BaseListingPageViewModel
    {
        public OrdersResponseViewModel()
        {
            if (Orders == null) Orders = new List<OrderResponseViewModel>();
        }
        public IEnumerable<OrderResponseViewModel> Orders { get; set; }
    }
}

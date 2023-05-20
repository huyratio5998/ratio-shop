namespace RatioShop.Data.ViewModels
{
    public class ListProductViewModel : BaseListingPageViewModel
    {
        public ListProductViewModel()
        {
            if(Products == null) Products = new List<ProductViewModel>();
        }
        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}

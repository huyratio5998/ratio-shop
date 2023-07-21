namespace RatioShop.Data.ViewModels
{
    public class ListProductResponseViewModel : BaseListingPageViewModel
    {
        public ListProductResponseViewModel()
        {
            if(Products == null) Products = new List<ProductViewModel>();            
        }

        public IEnumerable<ProductViewModel> Products { get; set; }        
    }
}

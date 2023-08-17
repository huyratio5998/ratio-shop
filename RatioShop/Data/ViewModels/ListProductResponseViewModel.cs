namespace RatioShop.Data.ViewModels
{
    public class ListProductResponseViewModel : BaseListingPageViewModel
    {
        public ListProductResponseViewModel()
        {
            if(Products == null) Products = new List<ProductViewModel>();            
            if(Packages == null) Packages = new List<PackageViewModel>();
        }

        public IEnumerable<ProductViewModel> Products { get; set; }        
        public IEnumerable<PackageViewModel> Packages { get; set; }
    }
}

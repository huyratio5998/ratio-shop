using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite;
using RatioShop.Data.ViewModels.SearchViewModel;

namespace RatioShop.Data.ViewModels
{
    public class ListProductViewModel : BaseListingPageViewModel
    {
        public ListProductViewModel()
        {
            if(Products == null) Products = new List<ProductViewModel>();
            if (FilterSettings == null) FilterSettings = new FilterSettings();
        }

        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<PackageViewModel> Packages { get; set; }
        public FilterSettings FilterSettings { get; set; }

        public ProductListingSettingViewModel? PLPSettings { get; set; }
    }
}

using RatioShop.Data.ViewModels;

namespace RatioShop.Areas.Admin.Models
{
    public class ListPackageViewModel : BaseListingPageViewModel
    {
        public ListPackageViewModel()
        {
            if (Packages == null) Packages = new List<PackageViewModel>();
        }
        public List<PackageViewModel> Packages { get; set; }
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;

namespace RatioShop.Areas.Admin.Models
{
    public class ListSiteSettingViewModel : BaseListingPageViewModel
    {
        public ListSiteSettingViewModel()
        {
            if (SiteSettings == null) SiteSettings = new List<SiteSetting>();
        }
        public List<SiteSetting> SiteSettings { get; set; }
    }
}

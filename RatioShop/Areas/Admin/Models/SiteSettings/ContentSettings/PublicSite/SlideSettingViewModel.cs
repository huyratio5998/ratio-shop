using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Common;
using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite
{
    public class SlideSettingViewModel: BaseSettingViewModel
    {
        public List<SlideSettingItemViewModel>? Banners { get; set; }

        public string? BannersString { get; set; }
    }
}

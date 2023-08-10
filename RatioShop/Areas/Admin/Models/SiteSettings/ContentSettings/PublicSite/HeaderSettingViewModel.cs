using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Common;
using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite
{
    public class HeaderSettingViewModel: BaseSettingViewModel
    {
        public string? TopBioText { get; set; }
        public LinkItemViewModel ShopLogo { get; set; }
        public List<LinkItemViewModel>? Navigations { get; set; }
        public string? NavigationsStringValue { get; set; }
    }
}

using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Admin;
using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite;

namespace RatioShop.Areas.Admin.Models.SiteSettings
{
    public class SiteSettingDetailViewModel : BaseSettingDetailViewModel
    {        
        public string? SingleSetting { get; set; }
        public HeaderSettingViewModel? HeaderSetting { get; set; }
        public FooterSettingViewModel? FooterSetting { get; set; }
        public GeneralSettingViewModel? GeneralSetting { get; set; }
        public SEOSettingViewModel? SEOSetting { get; set; }
        public SlideSettingViewModel? SlideSetting { get; set; }
        public ProductListingSettingViewModel? ProductListingSetting { get; set; }
        public ProductDetailSettingViewModel? ProductDetailSetting { get; set; }
        public AdminGeneralSettingViewModel? AdminGeneralSetting { get; set; }        
    }
}

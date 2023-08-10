using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Admin;
using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite;

namespace RatioShop.Areas.Admin.Models.SiteSettings
{
    public class SiteSettingViewModel
    {                
        public HeaderSettingViewModel? HeaderSetting { get; set; }
        public SlideSettingViewModel? HeaderSlides { get; set; }
        public FooterSettingViewModel? FooterSetting { get; set; }
        public GeneralSettingViewModel? GeneralSetting { get; set; }
        public SEOSettingViewModel? SEOSetting { get; set; }        

        // admin
        public HeaderSettingViewModel? AdminHeaderSetting { get; set; }
        public FooterSettingViewModel? AdminFooterSetting { get; set; }
        public AdminGeneralSettingViewModel? AdminGeneralSetting { get; set; }        
    }
}

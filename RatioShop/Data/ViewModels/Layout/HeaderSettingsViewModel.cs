using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite;

namespace RatioShop.Data.ViewModels.Layout
{
    public class HeaderSettingsViewModel
    {
        public HeaderSettingViewModel? HeaderSetting { get; set; }
        public SlideSettingViewModel? HeaderSlides { get; set; }
        public bool IsHideSilder { get; set; }        
    }
}

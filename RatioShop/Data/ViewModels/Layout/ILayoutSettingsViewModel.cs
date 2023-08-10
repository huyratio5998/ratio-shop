using RatioShop.Areas.Admin.Models.SiteSettings;

namespace RatioShop.Data.ViewModels.Layout
{
    public interface ILayoutSettingsViewModel
    {        
        string StoreName { get;}
        string StoreIcon { get; }        
        string StoreLogo { get; }
        SiteSettingViewModel SiteSettings { get; }
        HeaderSettingsViewModel HeaderSettings();
        CommonSettingsViewModel CommonSettings();
        FooterSettingsViewModel FooterSettings();        
    }
}

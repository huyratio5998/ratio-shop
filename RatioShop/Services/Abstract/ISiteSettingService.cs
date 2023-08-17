using RatioShop.Areas.Admin.Models;
using RatioShop.Areas.Admin.Models.SiteSettings;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.SearchViewModel;

namespace RatioShop.Services.Abstract
{
    public interface ISiteSettingService
    {
        IEnumerable<SiteSetting> GetSiteSettings();
        SiteSetting? GetSiteSetting(string id);
        Task<SiteSetting> CreateSiteSetting(SiteSetting SiteSetting);
        bool UpdateSiteSetting(SiteSetting SiteSetting);
        bool DeleteSiteSetting(string id);

        public ListSiteSettingViewModel GetSiteSettings(BaseSearchRequest args);
        SiteSettingDetailViewModel GetSetting(Guid id);
        Task<SiteSettingDetailViewModel>? GetSetting(string settingKey);
        Task<SiteSettingViewModel>? GetSiteSetting(bool isAdminSite = false);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ISiteSettingRepository : IBaseRepository<SiteSetting>
    {
        IQueryable<SiteSetting> GetSiteSettings();
        IQueryable<SiteSetting> GetSiteSettings(int pageIndex, int pageSize);
        SiteSetting? GetSiteSetting(string id);
        Task<SiteSetting> CreateSiteSetting(SiteSetting SiteSetting);
        bool UpdateSiteSetting(SiteSetting SiteSetting);
        bool DeleteSiteSetting(string id);
    }
}

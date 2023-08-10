using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class SiteSettingRepository : BaseProductRepository<SiteSetting>, ISiteSettingRepository
    {
        public SiteSettingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SiteSetting> CreateSiteSetting(SiteSetting SiteSetting)
        {
            return await Create(SiteSetting);
        }

        public bool DeleteSiteSetting(string id)
        {
            return Delete(id);
        }

        public IQueryable<SiteSetting> GetSiteSettings()
        {            
            return GetAll();
        }

        public IQueryable<SiteSetting> GetSiteSettings(int pageIndex, int pageSize)
        {
            return GetAll(pageIndex, pageSize);
        }

        public SiteSetting? GetSiteSetting(string id)
        {
            return GetById(id);
        }

        public bool UpdateSiteSetting(SiteSetting SiteSetting)
        {
            return Update(SiteSetting);
        }
    }
}

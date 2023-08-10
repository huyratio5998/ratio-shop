using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models;
using RatioShop.Areas.Admin.Models.SiteSettings;
using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Admin;
using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public SiteSettingService(ISiteSettingRepository siteSettingRepository, IMapper mapper, IMemoryCache memoryCache)
        {
            _siteSettingRepository = siteSettingRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public Task<SiteSetting> CreateSiteSetting(SiteSetting SiteSetting)
        {
            SiteSetting.CreatedDate = DateTime.UtcNow;
            SiteSetting.ModifiedDate = DateTime.UtcNow;
            return _siteSettingRepository.CreateSiteSetting(SiteSetting);
        }

        public bool DeleteSiteSetting(string id)
        {
            return _siteSettingRepository.DeleteSiteSetting(id);
        }

        public IEnumerable<SiteSetting> GetSiteSettings()
        {
            return _siteSettingRepository.GetSiteSettings();
        }

        public SiteSetting? GetSiteSetting(string id)
        {
            return _siteSettingRepository.GetSiteSetting(id);
        }

        public bool UpdateSiteSetting(SiteSetting SiteSetting)
        {
            SiteSetting.ModifiedDate = DateTime.UtcNow;
            return _siteSettingRepository.UpdateSiteSetting(SiteSetting);
        }

        public ListSiteSettingViewModel GetSiteSettings(BaseSearchRequest args)
        {
            if (args == null) return new ListSiteSettingViewModel();

            var siteSettings = _siteSettingRepository.GetSiteSettings();
            siteSettings = BuildSiteSettingFilters(siteSettings, args);
            siteSettings = siteSettings?.OrderByDescending(x => x.IsActive).ThenBy(x => x.Name);

            var totalCount = siteSettings?.Count() ?? 0;
            siteSettings = siteSettings?.PagingProductsGeneric(args);

            return new ListSiteSettingViewModel
            {
                SiteSettings = siteSettings?.ToList() ?? new List<SiteSetting>(),
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
        }

        private IQueryable<SiteSetting>? BuildSiteSettingFilters(IQueryable<SiteSetting>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            var predicate = PredicateBuilder.True<SiteSetting>();
            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.Text:
                        {
                            switch (item.FieldName)
                            {
                                case "":
                                    break;
                                case "Type":
                                    {
                                        var getSettingType = Enum.TryParse(typeof(SiteSettingType), item.Value, true, out var settingFilterType);
                                        if (!getSettingType) continue;
                                        predicate = predicate.And(x => x.Type == (SiteSettingType)settingFilterType);
                                        break;
                                    }
                                case "Name":
                                    predicate = predicate.And(x => x.Name.Contains(item.Value));
                                    break;
                                case "SettingTemplate":
                                    predicate = predicate.And(x => x.SettingTemplate.Contains(item.Value));
                                    break;
                            }
                            break;
                        }
                }
            }

            return queries.Where(predicate);
        }

        public SiteSettingDetailViewModel GetSetting(Guid id)
        {
            var setting = GetSiteSetting(id.ToString());
            if (setting == null) return new SiteSettingDetailViewModel();

            var result = _mapper.Map<SiteSettingDetailViewModel>(setting);
            switch (setting.Type)
            {
                case SiteSettingType.ItemSetting:
                    result.SingleSetting = setting.Value;
                    break;
                case SiteSettingType.GroupSettings:
                    result = MapSiteSettingsValue(ref result, setting.Value);
                    break;
            }

            return result;
        }

        private SiteSettingDetailViewModel MapSiteSettingsValue(ref SiteSettingDetailViewModel result, string settingValue)
        {
            switch (result.SettingTemplate)
            {
                case SiteSettings.SettingTemplates.Header:
                    result.HeaderSetting = JsonConvert.DeserializeObject<HeaderSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.Footer:
                    result.FooterSetting = JsonConvert.DeserializeObject<FooterSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.General:
                    result.GeneralSetting = JsonConvert.DeserializeObject<GeneralSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.SEO:
                    result.SEOSetting = JsonConvert.DeserializeObject<SEOSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.Slide:
                    result.SlideSetting = JsonConvert.DeserializeObject<SlideSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.ProductListing:
                    result.ProductListingSetting = JsonConvert.DeserializeObject<ProductListingSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.ProductDetail:
                    result.ProductDetailSetting = JsonConvert.DeserializeObject<ProductDetailSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.AdminGeneral:
                    result.AdminGeneralSetting = JsonConvert.DeserializeObject<AdminGeneralSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.AdminHeader:
                    result.HeaderSetting = JsonConvert.DeserializeObject<HeaderSettingViewModel>(settingValue);
                    break;
                case SiteSettings.SettingTemplates.AdminFooter:
                    result.FooterSetting = JsonConvert.DeserializeObject<FooterSettingViewModel>(settingValue);
                    break;
                default:
                    break;
            }

            return result;
        }

        public async Task<SiteSettingDetailViewModel>? GetSetting(string settingKey)
        {
            if (string.IsNullOrEmpty(settingKey)) return null;

            var setting = await _siteSettingRepository.GetSiteSettings().FirstOrDefaultAsync(x => x.Name == settingKey && x.IsActive);
            var result = _mapper.Map<SiteSettingDetailViewModel>(setting);

            if (setting == null || result == null) return null;

            switch (setting.Type)
            {
                case SiteSettingType.ItemSetting:
                    result.SingleSetting = setting.Value;
                    break;
                case SiteSettingType.GroupSettings:
                    result = MapSiteSettingsValue(ref result, setting.Value);
                    break;
            }

            return result;
        }

        public async Task<SiteSettingViewModel>? GetSiteSetting(bool isAdminSite = false)
        {
            string cacheKey = $"GetSiteSetting-{isAdminSite.ToString()}";

            if (!_memoryCache.TryGetValue(cacheKey, out SiteSettingViewModel result))
            {
                try
                {
                    await semaphoreSlim.WaitAsync();
                    if (!_memoryCache.TryGetValue(cacheKey, out result))
                    {
                        string SEOSettingKey = "seo-setting";
                        string headerSettingKey = "header-setting";
                        string headerSlidesSettingKey = "banner-setting";
                        string footerSettingKey = "footer-setting";
                        string generalSettingKey = "general-setting";

                        string adminHeaderSettingKey = "admin-header-setting";
                        string adminFooterSettingKey = "admin-footer-setting";
                        string adminGeneralSettingKey = "admin-general-setting";

                        result = new SiteSettingViewModel();

                        if (!isAdminSite)
                        {
                            result.SEOSetting = (await GetSetting(SEOSettingKey))?.SEOSetting;
                            result.HeaderSetting = (await GetSetting(headerSettingKey))?.HeaderSetting;
                            result.HeaderSlides = (await GetSetting(headerSlidesSettingKey))?.SlideSetting;
                            result.FooterSetting = (await GetSetting(footerSettingKey))?.FooterSetting;
                            result.GeneralSetting = (await GetSetting(generalSettingKey))?.GeneralSetting;
                        }
                        else
                        {
                            result.AdminHeaderSetting = (await GetSetting(adminHeaderSettingKey))?.HeaderSetting;
                            result.AdminFooterSetting = (await GetSetting(adminFooterSettingKey))?.FooterSetting;
                            result.AdminGeneralSetting = (await GetSetting(adminGeneralSettingKey))?.AdminGeneralSetting;
                        }

                        var cacheOption = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024); 

                        _memoryCache.Set(cacheKey, result, cacheOption);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return result;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,Shipper,Employee")]
    public class CacheManagerController : Controller
    {
        private IMemoryCache _memoryCache;

        public CacheManagerController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCache(string cacheKeys)
        {            
            if (string.IsNullOrEmpty(cacheKeys))
            {
                return BadRequest(false);
            }

            var listCachKeys = cacheKeys.Split(",");
            foreach (var cacheKey in listCachKeys)
            {
                if (string.IsNullOrWhiteSpace(cacheKey)) continue;
                try
                {
                    switch (cacheKey)
                    {
                        case "site-setting":
                            {
                                string SEOSettingKey = "seo-setting";
                                string headerSettingKey = "header-setting";
                                string headerSlidesSettingKey = "banner-setting";
                                string footerSettingKey = "footer-setting";
                                string generalSettingKey = "general-setting";
                                string baseKey = "cache";

                                _memoryCache.Remove("cache-common-setting-false");
                                _memoryCache.Remove($"{baseKey}-{SEOSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{headerSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{headerSlidesSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{footerSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{generalSettingKey}");                                
                                break;
                            }
                        case "admin-site-setting":
                            {
                                string adminHeaderSettingKey = "admin-header-setting";
                                string adminFooterSettingKey = "admin-footer-setting";
                                string adminGeneralSettingKey = "admin-general-setting";
                                string baseKey = "cache";

                                _memoryCache.Remove("cache-common-setting-true");
                                _memoryCache.Remove($"{baseKey}-{adminHeaderSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{adminFooterSettingKey}");
                                _memoryCache.Remove($"{baseKey}-{adminGeneralSettingKey}");
                                break;
                            }                        
                        default:
                            {
                                _memoryCache.Remove($"cache-{cacheKey}");
                                break;
                            }
                    }                    
                }
                catch (Exception ex)
                {
                    return Ok(false);
                }
            }

            return Ok(true);
        }
    }
}

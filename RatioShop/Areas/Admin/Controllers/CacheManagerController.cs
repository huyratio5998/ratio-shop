using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RatioShop.Constants;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,ContentEditor")]
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
                        case CacheConstant.SiteSettingKey:
                            {
                                _memoryCache.Remove($"{CacheConstant.CacheCommonSetting}-false");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.SEOSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.HeaderSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.HeaderSlidesSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.FooterSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.GeneralSettingKey}");
                                break;
                            }
                        case CacheConstant.AdminSiteSettingKey:
                            {
                                _memoryCache.Remove($"{CacheConstant.CacheCommonSetting}-true");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.AdminHeaderSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.AdminFooterSettingKey}");
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{CacheConstant.AdminGeneralSettingKey}");
                                break;
                            }
                        case CacheConstant.PDPKey:
                            {
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{cacheKey}");
                                CacheConstant.PDPCancellation.Cancel();
                                CacheConstant.PDPCancellation = new CancellationTokenSource();
                                break;
                            }
                        case CacheConstant.PLPKey:
                            {
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{cacheKey}");
                                CacheConstant.PLPCancellation.Cancel();
                                CacheConstant.PLPCancellation = new CancellationTokenSource();
                                break;
                            }
                        default:
                            {
                                _memoryCache.Remove($"{CacheConstant.BaseCache}-{cacheKey}");
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

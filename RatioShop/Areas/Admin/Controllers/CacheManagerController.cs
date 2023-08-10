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

        public IActionResult Index(string message="")
        {
            ViewBag.Message = message;
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
                try
                {
                    switch (cacheKey)
                    {
                        case "site-setting":
                            {
                                _memoryCache.Remove("GetSiteSetting-False");
                                break;
                            }
                        case "admin-site-setting":
                            {
                                _memoryCache.Remove("GetSiteSetting-True");
                                break;
                            }
                        default:
                            break;
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

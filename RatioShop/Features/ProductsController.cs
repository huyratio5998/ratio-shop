using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
using RatioShop.Services.Abstract;
using RatioShop.Services.Implement;

namespace RatioShop.Features
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IProductVariantStockService _productVariantStockService;
        private readonly IPackageService _packageService;
        private readonly ICommonService _commonService;
        private readonly IMemoryCache _memoryCache;
        private readonly ISiteSettingService _siteSettingService;

        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private const int pageSizeDefault = 5;
        private const int pageSizeClientDesktopDefault = 8;
        private const int pageSizeClientMobileDefault = 3;
        private const int maxRelatedNumber = 8;
        private const string productSettingKey = CacheConstant.PLPKey;

        public ProductsController(IProductService productService, IWebHostEnvironment hostingEnvironment, IProductVariantService productVariantService, IProductCategoryService productCategoryService, ICategoryService categoryService, IProductVariantStockService productVariantStockService, ICommonService commonService, IPackageService packageService, IMemoryCache memoryCache, ISiteSettingService siteSettingService)
        {
            this._productService = productService;
            _hostingEnvironment = hostingEnvironment;
            _productVariantService = productVariantService;
            _productCategoryService = productCategoryService;
            _categoryService = categoryService;
            _productVariantStockService = productVariantStockService;
            _commonService = commonService;
            _packageService = packageService;
            _memoryCache = memoryCache;
            _siteSettingService = siteSettingService;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index(ProductSearchRequest request)
        {
            var productSettings = (await _siteSettingService.GetSetting(productSettingKey))?.ProductListingSetting;
            var pageSizeDesktop = productSettings != null ? productSettings.DekstopPageSize ?? pageSizeClientDesktopDefault : pageSizeClientDesktopDefault;
            var pageSizeMobile = productSettings != null ? productSettings.MobilePageSize ?? pageSizeClientMobileDefault : pageSizeClientMobileDefault;

            request.PageSize = CommonHelper.GetClientDevice(Request) == Enums.DeviceType.Desktop ? pageSizeDesktop : pageSizeMobile;
            request.IsSelectPreviousItems = true;

            var productCacheKey = $"{CacheConstant.Products}-{JsonConvert.SerializeObject(request)}";
            if (!_memoryCache.TryGetValue(productCacheKey, out ListProductViewModel listProductViewModel))
            {
                try
                {
                    // use to prevent multi user save list to cache. Make sure only 1 does.
                    await semaphoreSlim.WaitAsync();
                    if (!_memoryCache.TryGetValue(productCacheKey, out listProductViewModel))
                    {
                        listProductViewModel = _productService.GetListProducts(request);
                        listProductViewModel.FilterSettings = new FilterSettings
                        {
                            PriceRangeFilter = _productService.GetProductPriceFilter(5000000, 5),
                            CategoryFilter = (productSettings != null && productSettings.CategoriesFilter != null && productSettings.CategoriesFilter.Any()) ? productSettings?.CategoriesFilter : SiteSettings.CategoriesFilter,
                            IsPackageView = request.IsGetPackages
                        };
                        listProductViewModel.PLPSettings = productSettings;

                        var cacheOption = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1024)
                            .AddExpirationToken(new CancellationChangeToken(CacheConstant.PLPCancellation.Token));

                        _memoryCache.Set(productCacheKey, listProductViewModel, cacheOption);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            // for share value of search on header partial
            ViewBag.Search = QueryableHelpers.GetFreeTextFilter(listProductViewModel?.FilterItems);
            ViewBag.SortType = request.SortType;

            return View(listProductViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string search = "", string sortType = "default", int page = 1)
        {
            var filterItems = JsonConvert.SerializeObject(
                    new List<FacetFilterItem>() {
                        new FacetFilterItem
                        {
                            FieldName = FieldNameFilter.Name.ToString(),
                            Type = FilterType.FreeText.ToString(),
                            Value = search
                        }
                    });

            return RedirectToAction("Index", new { filterItems = filterItems, sortType = sortType, page = page });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(Guid? id)
        {
            if (id == null) return NotFound();

            var productCacheKey = $"{CacheConstant.Products}-{id}";
            if (!_memoryCache.TryGetValue(productCacheKey, out ProductViewModel product))
            {
                try
                {
                    // use to prevent multi user save list to cache. Make sure only 1 does.
                    await semaphoreSlim.WaitAsync();
                    if (!_memoryCache.TryGetValue(productCacheKey, out product))
                    {
                        product = _productService.GetProduct((Guid)id);
                        product.RelatedProducts = _productService.GetRelatedProducts(product, maxRelatedNumber);
                        product.BreadCrumbs = _commonService.GetBreadCrumbsByProductId((Guid)id);

                        var cacheOption = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1024)
                            .AddExpirationToken(new CancellationChangeToken(CacheConstant.PDPCancellation.Token));

                        _memoryCache.Set(productCacheKey, product, cacheOption);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            if (product == null) return NotFound();

            return View("~/Views/Products/ProductDetail.cshtml", product);
        }
    }
}

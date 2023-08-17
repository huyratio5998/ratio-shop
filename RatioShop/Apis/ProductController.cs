using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public ProductController(IProductService productService, IMapper mapper, IMemoryCache memoryCache)
        {
            _productService = productService;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
        {
            if (ModelState.IsValid)
            {
                var productCacheKey = $"{CacheConstant.ProductsApi}-{JsonConvert.SerializeObject(request)}";
                if (!_memoryCache.TryGetValue(productCacheKey, out ListProductResponseViewModel responseResult))
                {
                    try
                    {
                        // use to prevent multi user save list to cache. Make sure only 1 does.
                        await semaphoreSlim.WaitAsync();
                        if (!_memoryCache.TryGetValue(productCacheKey, out responseResult))
                        {
                            var products = _productService.GetListProducts(request);
                            responseResult = _mapper.Map<ListProductResponseViewModel>(products);

                            var cacheOption = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                                .SetPriority(CacheItemPriority.High)
                                .SetSize(1024)
                                .AddExpirationToken(new CancellationChangeToken(CacheConstant.PLPCancellation.Token));

                            _memoryCache.Set(productCacheKey, responseResult, cacheOption);
                        }
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }

                return Ok(responseResult);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("detail/{productId:guid}")]
        public async Task<IActionResult> GetProductDetail([FromRoute] Guid productId)
        {
            if (productId == Guid.Empty) return NotFound();

            var productCacheKey = $"{CacheConstant.ProductApi}-{productId}";
            if (!_memoryCache.TryGetValue(productCacheKey, out ProductViewModel product))
            {
                try
                {
                    // use to prevent multi user save list to cache. Make sure only 1 does.
                    await semaphoreSlim.WaitAsync();
                    if (!_memoryCache.TryGetValue(productCacheKey, out product))
                    {
                        product = _productService.GetProduct(productId);

                        var cacheOption = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                            .SetPriority(CacheItemPriority.Normal)
                            .AddExpirationToken(new CancellationChangeToken(CacheConstant.PDPCancellation.Token));

                        _memoryCache.Set(productCacheKey, product, cacheOption);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return Ok(product);
        }
    }
}

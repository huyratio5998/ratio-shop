using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetProducts([FromQuery] string? search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var sortBy = "default";
            var products = _productService.GetListProducts(search, sortBy, page, pageSize);

            return Ok(products);
        }

        [HttpGet]
        [Route("detail/{productId:guid}")]
        public IActionResult GetProductDetail([FromRoute] Guid productId)
        {
            if (productId == Guid.Empty) return NotFound();

            var product = _productService.GetProduct(productId);
            return Ok(product);
        }
    }
}

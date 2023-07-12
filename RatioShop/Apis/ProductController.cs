using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Data.ViewModels;
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
        public IActionResult GetProducts([FromQuery] int page, [FromQuery] int pageSize)
        {
            string sortBy = "default";
            var products = _productService.GetProducts(sortBy, page, pageSize).ToList();
            products = _productService.GetProductsRelatedInformation(products);

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

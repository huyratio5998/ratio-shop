using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetProducts([FromQuery] ProductSearchRequest request)
        {
            if (ModelState.IsValid)
            {
                var sortBy = "default";
                var products = _productService.GetListProducts(request);

                return Ok(products);
            }
            else
            {
                return BadRequest();
            }
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

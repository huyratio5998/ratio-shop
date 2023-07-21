﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetProducts([FromQuery] ProductSearchRequest request)
        {
            if (ModelState.IsValid)
            {                
                var products = _productService.GetListProducts(request);
                var responseResult = _mapper.Map<ListProductResponseViewModel>(products);

                return Ok(responseResult);
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

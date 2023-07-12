using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers;
using RatioShop.Helpers.FileHelpers;
using RatioShop.Services.Abstract;

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
        
        private const int pageSizeDefault = 5;
        private const int pageSizeClientDesktopDefault = 8;
        private const int pageSizeClientMobileDefault = 3;

        public ProductsController(IProductService productService, IWebHostEnvironment hostingEnvironment, IProductVariantService productVariantService, IProductCategoryService productCategoryService, ICategoryService categoryService, IProductVariantStockService productVariantStockService)
        {
            this._productService = productService;
            _hostingEnvironment = hostingEnvironment;
            _productVariantService = productVariantService;
            _productCategoryService = productCategoryService;
            _categoryService = categoryService;
            _productVariantStockService = productVariantStockService;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortBy = "default", int page = 1)
        {
            
            var listProductViewModel = new ListProductViewModel();
            var pageSize = CommonHelper.GetClientDevice(Request) == Enums.DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var listProducts = _productService.GetAllProductsByPageNumber(sortBy, page, pageSize).ToList();
            listProducts = _productService.GetProductsRelatedInformation(listProducts);

            listProductViewModel.Products = listProducts;
            
            //paging information
            listProductViewModel.PageIndex = page;
            listProductViewModel.PageSize = pageSize;
            listProductViewModel.TotalCount = _productService.GetProducts().Count();
            listProductViewModel.TotalPage = listProductViewModel.TotalCount == 0 ? 1 : (int)Math.Ceiling((double)listProductViewModel.TotalCount / pageSize);
            //
            ViewBag.SortBy = sortBy;
            ViewBag.Page = page;

            return View(listProductViewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(Guid? id)
        {
            if (id == null) return NotFound();

            var product = _productService.GetProduct((Guid)id);

            if (product == null) return NotFound();

            return View("~/Views/Products/ProductDetail.cshtml", product);
        }

        //Admin
        public async Task<IActionResult> ProductSettings(string sortBy = "default", int page = 1)
        {
            var listProductViewModel = new ListProductViewModel();
            var listProducts = _productService.GetProducts(sortBy, page, pageSizeDefault).ToList();
            listProductViewModel.Products = listProducts;

            // get variants, categories
            foreach (var item in listProducts)
            {
                item.Product.Variants = _productVariantService.GetProductVariantsByProductId(item.Product.Id).ToList();
                item.ProductCategories = _categoryService.GetCategorysByProductId(item.Product.Id);
            }
            //paging information
            listProductViewModel.PageIndex = page;
            listProductViewModel.PageSize = pageSizeDefault;
            listProductViewModel.TotalCount = _productService.GetProducts().Count();
            listProductViewModel.TotalPage = listProductViewModel.TotalCount == 0 ? 1 : (int)Math.Ceiling((double)listProductViewModel.TotalCount / pageSizeDefault);
            //
            ViewBag.SortBy = sortBy;
            ViewBag.Page = page;

            return View(listProductViewModel);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var product = _productService.GetProduct((Guid)id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productEntity = model.Product;
                productEntity.Id = Guid.NewGuid();

                FileHelpers.UploadFile(model.ProductImage, _hostingEnvironment, "images", "products");
                productEntity.ProductImage = model.ProductImage?.FileName;

                await _productService.AddProduct(productEntity);                

                return RedirectToAction(nameof(ProductSettings));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var product = _productService.GetProduct((Guid)id);                        

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, ProductViewModel model)
        {
            if (model == null || model.Product == null || id != model.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var productEntity = model.Product;

                FileHelpers.UploadFile(model.ProductImage, _hostingEnvironment, "images", "products");
                if (model.ProductImage != null) productEntity.ProductImage = model.ProductImage.FileName;

                _productService.UpdateProduct(productEntity);

                return RedirectToAction(nameof(ProductSettings));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var product = _productService.GetProduct((Guid)id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = _productService.DeleteProduct(id);

            return RedirectToAction(nameof(ProductSettings));
        }

        [HttpPost]
        public async Task<string> UpdateProductAdditionalInformation([FromBody]UpdateProductAdditionalInformationRequest data)
        {
            try
            {
                if (data == null) return JsonConvert.SerializeObject(new UpdateProductAdditionalInformationResponse(false, null));
                //remove variant
                foreach (var variantId in data.RemoveVariants)
                {
                    _productVariantService.DeleteProductVariant(variantId);
                }
                foreach (var productCategoryItem in data.RemoveProductCategories)
                {
                    _productCategoryService.DeleteProductCategory(productCategoryItem, data.ProductId);
                }
                // add or update variant
                foreach (var item in data.Variants)
                {
                    var variant = new ProductVariant()
                    {
                        Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
                        Code = item.Code,
                        Number = item.Number,
                        Price = item.Price,
                        DiscountRate = item.DiscountRate,
                        ProductId = data.ProductId
                    };

                    var productVariant = _productVariantService.GetProductVariant(variant.Id.ToString());
                    if (productVariant == null)
                    {
                        await _productVariantService.CreateProductVariant(variant);
                        _productVariantStockService.CreateOrUpdateProductVariantStock(true, variant.Id, item.ProductVariantStocks);
                    }                        
                    else
                    {
                        variant.CreatedDate = productVariant.CreatedDate;
                        _productVariantService.UpdateProductVariant(variant);
                        _productVariantStockService.CreateOrUpdateProductVariantStock(false, variant.Id, item.ProductVariantStocks);
                    }                        
                }
                // add or update product categories
                foreach (var item in data.ProductCategories)
                {
                    var productCategory = new ProductCategory()
                    {                                               
                        ProductId = data.ProductId,
                        CategoryId = item.Id                        
                    };

                    var productVariant = _productCategoryService.GetProductCategory(productCategory.CategoryId, productCategory.ProductId);
                    if (productVariant == null)
                        await _productCategoryService.CreateProductCategory(productCategory);
                    else
                    {
                        _productCategoryService.UpdateProductCategory(productCategory);
                    }
                }

                var response = new UpdateProductAdditionalInformationResponse(true, _productVariantService.GetProductVariantsByProductId(data.ProductId).ToList());
                
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new UpdateProductAdditionalInformationResponse(false, null));
            }
        }
    }
}

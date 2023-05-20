using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers.FileHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Features
{
    public class ProductsController : Controller
    {        
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private const int pageSizeDefault = 5;

        public ProductsController(IProductService productService, IWebHostEnvironment hostingEnvironment)
        {
            this._productService = productService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortBy= "default", int page=1)
        {
            var listProductViewModel = new ListProductViewModel();            
            listProductViewModel.Products = _productService.GetProducts(sortBy, page, pageSizeDefault);
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

        public async Task<IActionResult> ProductSettings(string sortBy = "default", int page = 1)
        {
            var listProductViewModel = new ListProductViewModel();
            listProductViewModel.Products = _productService.GetProducts(sortBy, page, pageSizeDefault);
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
                
                return RedirectToAction(nameof(Index));
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
                if(model.ProductImage != null) productEntity.ProductImage =  model.ProductImage.FileName;
                
                _productService.UpdateProduct(productEntity);

                return RedirectToAction(nameof(Index));
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
            
            return RedirectToAction(nameof(Index));
        }       
    }
}

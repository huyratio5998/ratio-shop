using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Services.Abstract;
using System.Data;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,ContentEditor")]
    public class ProductVariantsController : Controller
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IProductService _productService;

        public ProductVariantsController(IProductVariantService productVariantService, IProductService productService)
        {
            _productVariantService = productVariantService;
            _productService = productService;
        }

        // GET: ProductVariants
        public async Task<IActionResult> Index()
        {
            var results = _productVariantService.GetProductVariants();
            return View(results);
        }

        // GET: ProductVariants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = _productVariantService.GetProductVariant(id.ToString());

            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // GET: ProductVariants/Create
        public IActionResult Create()
        {
            var products = _productService.GetProducts().Select(x => x.Product);

            ViewData["ProductId"] = new SelectList(products, "Id", "Id");

            return View();
        }

        // POST: ProductVariants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Number,Price,DiscountRate,ProductId,Id,CreatedDate,ModifiedDate")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                await _productVariantService.CreateProductVariant(productVariant);

                return RedirectToAction(nameof(Index));
            }

            var products = _productService.GetProducts().Select(x => x.Product);
            ViewData["ProductId"] = new SelectList(products, "Id", "Id", productVariant.ProductId);

            return View(productVariant);
        }

        // GET: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            var productVariant = _productVariantService.GetProductVariant(id.ToString());
            if (productVariant == null)
            {
                return NotFound();
            }

            var products = _productService.GetProducts().Select(x => x.Product);
            ViewData["ProductId"] = new SelectList(products, "Id", "Id", productVariant.ProductId);

            return View(productVariant);
        }

        // POST: ProductVariants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Code,Number,Price,DiscountRate,ProductId,Id,CreatedDate,ModifiedDate")] ProductVariant productVariant)
        {
            if (id != productVariant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _productVariantService.UpdateProductVariant(productVariant);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantExists(productVariant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var products = _productService.GetProducts().Select(x => x.Product);
            ViewData["ProductId"] = new SelectList(products, "Id", "Id", productVariant.ProductId);

            return View(productVariant);
        }

        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = _productVariantService.GetProductVariant(id.ToString());

            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            _productVariantService.DeleteProductVariant(id.ToString());

            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantExists(Guid id)
        {
            var productVariant = _productVariantService.GetProductVariant(id.ToString(), false);

            return productVariant != null;
        }
    }
}

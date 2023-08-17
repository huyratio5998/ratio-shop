using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RatioShop.Data;
using RatioShop.Data.Models;
using RatioShop.Services.Abstract;
using System.Data;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,ContentEditor")]
    public class ProductVariantStocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockService _stockService;
        private readonly IProductVariantService _productVariantService;
        private readonly IProductVariantStockService _productVariantStockService;
        private readonly IProductService _productService;

        public ProductVariantStocksController(ApplicationDbContext context, IStockService stockService, IProductVariantService productVariantService, IProductService productService, IProductVariantStockService productVariantStockService)
        {
            _context = context;
            _stockService = stockService;
            _productVariantService = productVariantService;
            _productService = productService;
            _productVariantStockService = productVariantStockService;
        }

        // GET: ProductVariantStocks
        public async Task<IActionResult> Index()
        {
            var productVariantStocks = _productVariantStockService.GetProductVariantStocks().ToList();
            if (productVariantStocks != null && productVariantStocks.Any())
            {
                foreach (var item in productVariantStocks)
                {
                    item.ProductVariant = _productVariantService.GetProductVariant(item.ProductVariantId.ToString());
                    item.Stock = _stockService.GetStock(item.StockId);
                }
            }
            return View(productVariantStocks?.OrderBy(x => x.ProductVariant?.ProductId));
        }

        // GET: ProductVariantStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductVariantStock == null)
            {
                return NotFound();
            }

            var productVariantStock = await _context.ProductVariantStock
                .Include(p => p.ProductVariant)
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(m => m.StockId == id);
            if (productVariantStock == null)
            {
                return NotFound();
            }

            return View(productVariantStock);
        }

        // GET: ProductVariantStocks/Create
        public IActionResult Create()
        {
            var productVariants = _productVariantService.GetProductVariants().ToList();
            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    item.Product = _productService.GetProduct(item.ProductId).Product;
                }
            }
            ViewData["ProductVariantId"] = new SelectList(productVariants?.Select(x => new { x.Id, x.Code, ProductCode = x.Product.Code, CodeDisplay = $"{x.Code} - product: {x.Product?.Name}" }).OrderBy(x => x.ProductCode), "Id", "CodeDisplay");
            ViewData["StockId"] = new SelectList(_stockService.GetStocks(), "Id", "Name");
            return View();
        }

        // POST: ProductVariantStocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantStock productVariantStock)
        {
            if (ModelState.IsValid)
            {
                await _productVariantStockService.CreateProductVariantStock(productVariantStock);
                return RedirectToAction(nameof(Index));
            }
            // return view if invalid
            var productVariants = _productVariantService.GetProductVariants().ToList();
            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    item.Product = _productService.GetProduct(item.ProductId).Product;
                }
            }
            ViewData["ProductVariantId"] = new SelectList(productVariants?.Select(x => new { x.Id, x.Code, ProductCode = x.Product.Code, CodeDisplay = $"{x.Code} - product: {x.Product?.Name}" }).OrderBy(x => x.ProductCode), "Id", "CodeDisplay");
            ViewData["StockId"] = new SelectList(_stockService.GetStocks(), "Id", "Name");
            return View(productVariantStock);
        }

        // GET: ProductVariantStocks/Edit/5
        public async Task<IActionResult> Edit(int? stockId, Guid productVariantId)
        {
            if (stockId == null || productVariantId == Guid.Empty)
            {
                return NotFound();
            }

            var productVariantStock = _productVariantStockService.GetProductVariantStock((int)stockId, productVariantId);
            if (productVariantStock == null)
            {
                return NotFound();
            }

            // return view if invalid
            var productVariants = _productVariantService.GetProductVariants().ToList();
            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    item.Product = _productService.GetProduct(item.ProductId).Product;
                }
            }
            ViewData["ProductVariantId"] = new SelectList(productVariants?.Select(x => new { x.Id, x.Code, ProductCode = x.Product.Code, CodeDisplay = $"{x.Code} - product: {x.Product?.Name}" }).OrderBy(x => x.ProductCode), "Id", "CodeDisplay");
            ViewData["StockId"] = new SelectList(_stockService.GetStocks(), "Id", "Name");
            return View(productVariantStock);
        }

        // POST: ProductVariantStocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,ProductVariantId")] ProductVariantStock productVariantStock)
        {
            if (id != productVariantStock.StockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productVariantStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantStockExists(productVariantStock.StockId))
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
            ViewData["ProductVariantId"] = new SelectList(_context.ProductVariant, "Id", "Id", productVariantStock.ProductVariantId);
            ViewData["StockId"] = new SelectList(_context.Stock, "Id", "Id", productVariantStock.StockId);
            return View(productVariantStock);
        }

        // GET: ProductVariantStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductVariantStock == null)
            {
                return NotFound();
            }

            var productVariantStock = await _context.ProductVariantStock
                .Include(p => p.ProductVariant)
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(m => m.StockId == id);
            if (productVariantStock == null)
            {
                return NotFound();
            }

            return View(productVariantStock);
        }

        // POST: ProductVariantStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductVariantStock == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductVariantStock'  is null.");
            }
            var productVariantStock = await _context.ProductVariantStock.FindAsync(id);
            if (productVariantStock != null)
            {
                _context.ProductVariantStock.Remove(productVariantStock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantStockExists(int id)
        {
            return (_context.ProductVariantStock?.Any(e => e.StockId == id)).GetValueOrDefault();
        }
    }
}

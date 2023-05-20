using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RatioShop.Data;
using RatioShop.Data.Models;

namespace RatioShop.Features
{
    public class ProductVariantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductVariants
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductVariant.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductVariants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ProductVariant == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariant
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // GET: ProductVariants/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Id");
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
                productVariant.Id = Guid.NewGuid();
                _context.Add(productVariant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Id", productVariant.ProductId);
            return View(productVariant);
        }

        // GET: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ProductVariant == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariant.FindAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Id", productVariant.ProductId);
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
                    _context.Update(productVariant);
                    await _context.SaveChangesAsync();
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Id", productVariant.ProductId);
            return View(productVariant);
        }

        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ProductVariant == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariant
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.ProductVariant == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductVariant'  is null.");
            }
            var productVariant = await _context.ProductVariant.FindAsync(id);
            if (productVariant != null)
            {
                _context.ProductVariant.Remove(productVariant);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantExists(Guid id)
        {
          return (_context.ProductVariant?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

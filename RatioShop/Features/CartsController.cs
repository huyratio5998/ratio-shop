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
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cart.Include(c => c.ProductVariantCart).Include(c => c.ShopUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.ProductVariantCart)
                .Include(c => c.ShopUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["ProductVariantCartId"] = new SelectList(_context.Set<ProductVariantCart>(), "Id", "Id");
            ViewData["ShopUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Status,ProductVariantCartId,ShopUserId,Id,CreatedDate,ModifiedDate")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                cart.Id = Guid.NewGuid();
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductVariantCartId"] = new SelectList(_context.Set<ProductVariantCart>(), "Id", "Id", cart.ProductVariantCartId);
            ViewData["ShopUserId"] = new SelectList(_context.Users, "Id", "Id", cart.ShopUserId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductVariantCartId"] = new SelectList(_context.Set<ProductVariantCart>(), "Id", "Id", cart.ProductVariantCartId);
            ViewData["ShopUserId"] = new SelectList(_context.Users, "Id", "Id", cart.ShopUserId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Status,ProductVariantCartId,ShopUserId,Id,CreatedDate,ModifiedDate")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["ProductVariantCartId"] = new SelectList(_context.Set<ProductVariantCart>(), "Id", "Id", cart.ProductVariantCartId);
            ViewData["ShopUserId"] = new SelectList(_context.Users, "Id", "Id", cart.ShopUserId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.ProductVariantCart)
                .Include(c => c.ShopUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Cart == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cart'  is null.");
            }
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(Guid id)
        {
          return (_context.Cart?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

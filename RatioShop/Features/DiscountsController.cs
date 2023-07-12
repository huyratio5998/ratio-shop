using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Services.Abstract;

namespace RatioShop.Features
{
    public class DiscountsController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // GET: Discounts
        public async Task<IActionResult> Index()
        {
            var discounts = _discountService.GetDiscounts();
            return View(discounts);
        }

        // GET: Discounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = _discountService.GetDiscount((int)id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // GET: Discounts/Create
        public IActionResult Create()
        {
            ViewBag.Status = new SelectList(typeof(CommonStatus.Discount).GetFields().Select(x => x.GetValue(null)?.ToString()));
            return View();
        }

        // POST: Discounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,Number,Value,DiscountType,StartDate,ExpiredDate,Status,Id,CreatedDate,ModifiedDate")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                await _discountService.CreateDiscount(discount);
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // GET: Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Status = new SelectList(typeof(CommonStatus.Discount).GetFields().Select(x => x.GetValue(null)?.ToString()));
            var discount = _discountService.GetDiscount((int)id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Discounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,Number,Value,DiscountType,StartDate,ExpiredDate,Status,Id,CreatedDate,ModifiedDate")] Discount discount)
        {
            if (id != discount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _discountService.UpdateDiscount(discount);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.Id))
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
            return View(discount);
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = _discountService.GetDiscount((int)id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = _discountService.DeleteDiscount(id);
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _discountService.GetDiscount(id) != null;
        }
    }
}

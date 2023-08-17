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
    public class StocksController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IAddressService _addressService;

        public StocksController(IStockService stockService, IAddressService addressService)
        {
            _stockService = stockService;
            _addressService = addressService;
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            var stocks = _stockService.GetStocks().AsQueryable().Include(x => x.Address);

            return View(stocks);
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = _stockService.GetStock((int)id);

            if (stock == null)
            {
                return NotFound();
            }

            stock.Address = _addressService.GetAddress(stock.AddressId);

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            var address = _addressService.GetAddresses();
            ViewData["AddressId"] = new SelectList(address, "Id", "Id");

            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsActive,AddressId,Id,CreatedDate,ModifiedDate")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                await _stockService.CreateStock(stock);
                return RedirectToAction(nameof(Index));
            }

            var address = _addressService.GetAddresses();
            ViewData["AddressId"] = new SelectList(address, "Id", "Id", stock.AddressId);

            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = _stockService.GetStock((int)id);
            if (stock == null)
            {
                return NotFound();
            }

            var address = _addressService.GetAddresses();
            ViewData["AddressId"] = new SelectList(address, "Id", "Id", stock.AddressId);

            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,IsActive,AddressId,Id,CreatedDate,ModifiedDate")] Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _stockService.UpdateStock(stock);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id))
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
            var address = _addressService.GetAddresses();
            ViewData["AddressId"] = new SelectList(address, "Id", "Id", stock.AddressId);

            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = _stockService.GetStock((int)id);
            if (stock == null)
            {
                return NotFound();
            }

            stock.Address = _addressService.GetAddress(stock.AddressId);
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _stockService.DeleteStock(id);

            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            var stock = _stockService.GetStock(id);

            return stock != null;
        }
    }
}

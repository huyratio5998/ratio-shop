using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;        
        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterOrder(string? orderNumber, string? orderStatus, string? shipmentStatus, string? paymentType, SortingEnum? sortType, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            if (!string.IsNullOrWhiteSpace(orderNumber))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "OrderNumber", Type = FilterType.Text.ToString(), Value = orderNumber });
            if (!string.IsNullOrWhiteSpace(orderStatus))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Status", Type = FilterType.Text.ToString(), Value = orderStatus });
            if (!string.IsNullOrWhiteSpace(shipmentStatus))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "ShipmentStatus", Type = FilterType.Text.ToString(), Value = shipmentStatus });
            if (!string.IsNullOrWhiteSpace(paymentType))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "PaymentType", Type = FilterType.Text.ToString(), Value = paymentType });

            if (sortType == SortingEnum.Default) sortType = null;
            if (page <= 1) page = null;

            return RedirectToAction("Index", new { filterItems = listFilterItems.FilterItemToJson(), sortType = sortType, page = page });
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(BaseSearchArgs requestArgs)
        {
            // filter request            
            var request = _mapper.Map<BaseSearchRequest>(requestArgs);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;            

            ListOrderViewModel orders = _orderService.GetOrders(request);

            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }
            var orderDetail = _orderService.GetOrderDetailResponse(id.ToString(), false, true);            

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        //// GET: Admin/Orders/Create
        //public IActionResult Create()
        //{            
        //    return View();
        //}

        //// POST: Admin/Orders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderNumber,Status,TotalMoney,IsRefund,ShipmentStatus,ShipmentFee,CartId,PaymentId,Id,CreatedDate,ModifiedDate")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        order.Id = Guid.NewGuid();
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "Id", order.PaymentId);
        //    return View(order);
        //}

        //// GET: Admin/Orders/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "Id", order.PaymentId);
        //    return View(order);
        //}

        //// POST: Admin/Orders/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("OrderNumber,Status,TotalMoney,IsRefund,ShipmentStatus,ShipmentFee,CartId,PaymentId,Id,CreatedDate,ModifiedDate")] Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "Id", order.PaymentId);
        //    return View(order);
        //}

        //// GET: Admin/Orders/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order
        //        .Include(o => o.Payment)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //// POST: Admin/Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    if (_context.Order == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
        //    }
        //    var order = await _context.Order.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Order.Remove(order);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool OrderExists(Guid id)
        //{
        //  return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}

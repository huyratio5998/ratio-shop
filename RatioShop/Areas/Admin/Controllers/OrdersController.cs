using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager,SuperAdmin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;
        public OrdersController(IOrderService orderService, IMapper mapper, IShipmentService shipmentService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _shipmentService = shipmentService;
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

        public async Task<IActionResult> Index(BaseSearchArgs requestArgs)
        {
            // filter request            
            ViewBag.Area = "Admin";
            ViewBag.Controller = "Orders";
            ViewBag.Action = "Index";

            var request = _mapper.Map<BaseSearchRequest>(requestArgs);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;

            ListOrderViewModel orders = _orderService.GetOrders(request);

            // get shipper info.
            if (orders.Orders != null && orders.Orders.Any())
            {
                foreach (var item in orders.Orders)
                {
                    if (item.Order == null) continue;

                    var currentShipper = _shipmentService.GetShipments()
                        .Where(x => x.OrderId == item.Order.Id && x.UpdateStatus == true && !string.IsNullOrEmpty(x.ShipperId))
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();
                    item.AssignedShipper = currentShipper != null;
                    if (currentShipper != null) item.ShipperName = _shipmentService.GetShipperNameById(currentShipper.ShipperId);
                }
            }

            return View(orders);
        }

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

            if(orderDetail.ShipmentHistory != null) orderDetail.ShipmentHistory.AvailableShippers = _shipmentService.GetAvailableShippers();

            return View(orderDetail);
        }                
    }
}

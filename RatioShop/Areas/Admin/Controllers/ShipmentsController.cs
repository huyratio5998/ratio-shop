using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Areas.Admin.Models;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.FileHelpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShipmentsController : Controller
    {
        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private const int pageSizeClientDesktopDefault = 8;
        private const int pageSizeClientMobileDefault = 6;
        public ShipmentsController(IShipmentService shipmentService, IOrderService orderService, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _shipmentService = shipmentService;
            _orderService = orderService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> ShipmentHistory(Guid orderId)
        {
            if (orderId == Guid.Empty) return RedirectToAction("Index", "Orders");

            var orderDetail = _orderService.GetOrderDetailResponse(orderId.ToString());
            if (orderDetail == null) return RedirectToAction("Index", "Orders");

            if (orderDetail.ShipmentHistory != null) orderDetail.ShipmentHistory.AvailableShippers = _shipmentService.GetAvailableShippers();

            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterOrder(string actionRedirect, string? orderNumber, string? orderStatus, string? shipmentStatus, string? paymentType, SortingEnum? sortType, int? page = 1)
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

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), sortType = sortType, page = page });
        }

        [Route("admin/shipments/myShipment")]
        public IActionResult MyShipment(BaseSearchArgs requestArgs)
        {
            var result = MyShipmentHandler(requestArgs, false);
            if (result == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "Shipments";
            ViewBag.Action = "MyShipment";
            ViewBag.DetailParam = "MyShipment";
            ViewData["Title"] = "My Shipment Order";

            return View(result);
        }

        [Route("admin/shipments/myFinishedShipment")]
        public IActionResult MyFinishedShipment(BaseSearchArgs requestArgs)
        {
            var result = MyShipmentHandler(requestArgs, true);
            if (result == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "Shipments";
            ViewBag.Action = "MyFinishedShipment";
            ViewBag.DetailParam = "MyFinishedShipment";
            ViewData["Title"] = "My Finished Shipment Order";

            return View("~/Areas/Admin/Views/Shipments/MyShipment.cshtml", result);
        }

        [Route("admin/shipments/orders")]
        public IActionResult GetUnAssignedShipmentOrders(BaseSearchArgs requestArgs, string message = "")
        {
            var result = MyShipmentHandler(requestArgs, false, true);
            if (result == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "Shipments";
            ViewBag.Action = "GetUnAssignedShipmentOrders";
            ViewBag.DetailParam = "GetUnAssignedShipmentOrders";
            ViewData["Title"] = "List orders ready for shipment";

            ViewBag.AssignMessage = message;
            return View("~/Areas/Admin/Views/Shipments/MyShipment.cshtml", result);
        }

        [Route("admin/shipments/detail")]
        public IActionResult ShipmentDetail(Guid orderId, string detailView = "MyShipment")
        {
            if (User == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToAction("Index", "Home");

            if (orderId == Guid.Empty) return RedirectToAction("MyShipment", "Shipment");

            var orderDetail = _orderService.GetOrderDetailResponse(orderId.ToString());
            ViewBag.ShipperId = userId;
            ViewBag.DetailView = detailView;

            return View(orderDetail);
        }
        private ListShipmentViewModel? MyShipmentHandler(BaseSearchArgs requestArgs, bool getFinishedShipmentOrder = false, bool isGetAllUnAssignedOrders = false)
        {
            if (User == null || !User.Identity.IsAuthenticated) return null;

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return null;

            requestArgs.SortType = getFinishedShipmentOrder ? SortingEnum.RecentUpdate : SortingEnum.Oldest;

            var request = _mapper.Map<BaseSearchRequest>(requestArgs);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;

            var myInProgressOrders = isGetAllUnAssignedOrders ? _shipmentService.GetAllUnAssignedOrderIds()?.ToList() : getFinishedShipmentOrder ? _shipmentService.GetFinishedOrderIdsByShipperId(userId)?.ToList() : _shipmentService.GetInprogressOrderIdsByShipperId(userId)?.ToList();

            // apply filter
            var listOrderViewModel = _orderService.GetOrders(request, myInProgressOrders);
            var result = _mapper.Map<ListShipmentViewModel>(listOrderViewModel);

            var ordersFiltered = listOrderViewModel.Orders.Select(x => x.Order);
            if (ordersFiltered != null && ordersFiltered.Any())
            {
                foreach (var item in ordersFiltered)
                {
                    if (item == null) continue;

                    var orderDetail = _orderService.GetOrderDetailResponse(item, false, true);
                    if (orderDetail != null) result.Orders.Add(orderDetail);
                }
            }

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/shipments/shipmentTracking")]
        public async Task<IActionResult> ShipmentTracking(ShipmentTrackingRequestViewModel request)
        {
            if (request == null) return BadRequest();

            FileHelpers.UploadFile(request.FileImage, _hostingEnvironment, "images", "shipments", request.OrderNumber);
            if (request.FileImage != null) request.Images = request.FileImage.FileName;

            var result = await _orderService.ShipmentTracking(request);

            return RedirectToAction("ShipmentDetail", new { orderId = request.OrderId });
        }

        public IActionResult AssignOrderShipmentToShipper(Guid orderId, string orderNumber, string? shipperId = "")
        {
            if (orderId == null || string.IsNullOrEmpty(orderNumber)) return RedirectToAction("Index", "Home");

            var userId = shipperId;
            if (string.IsNullOrEmpty(userId))
            {
                if (User == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

                userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return RedirectToAction("Index", "Home");
            }

            var shipmentPending = _shipmentService.GetShipments().FirstOrDefault(x => x.OrderId == orderId && x.ShipmentStatus == CommonStatus.ShipmentStatus.Pending && x.UpdateStatus == true);
            if (shipmentPending == null) return RedirectToAction("Index", "Home");

            shipmentPending.ShipperId = userId;
            var updateStatus = _shipmentService.UpdateShipment(shipmentPending);

            if (!updateStatus) return RedirectToAction("Index", "Home");

            return RedirectToAction("GetUnAssignedShipmentOrders", new { message = orderNumber });
        }
    }
}

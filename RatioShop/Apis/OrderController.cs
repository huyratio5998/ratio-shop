using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.OrdersViewModel;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 6;
        private const int pageSizeClientMobileDefault = 5;
        public OrderController(IOrderService orderService, IShipmentService shipmentService, IPaymentService paymentService, IMapper mapper)
        {
            _orderService = orderService;
            _shipmentService = shipmentService;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [Route("detail")]
        public IActionResult GetOrderDetail([FromQuery] Guid id, [FromQuery] bool getLatestVariantPrice = true, [FromQuery] bool isIncludeInActiveDiscount = false)
        {
            if (id == Guid.Empty) return BadRequest();

            // get order
            var orderDetail = _orderService.GetOrderDetailResponse(id.ToString(), getLatestVariantPrice, isIncludeInActiveDiscount);
            if (orderDetail == null) return NotFound();
            return Ok(orderDetail);
        }

        [Route("getOrderHistory")]
        public async Task<IActionResult> GetOrderHistory(int page = 1)
        {
            if (User == null || !User.Identity.IsAuthenticated) return NotFound();

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();


            var orderPageSize = CommonHelper.GetClientDevice(Request) == Enums.DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var totalOrderByUser = _orderService.GetTotalOrderByUserId(userId);
            ListOrderViewModel orderHistories = _orderService.GetOrderHistoryByUserId(userId, page, orderPageSize);

            var result = _mapper.Map<OrdersResponseViewModel>(orderHistories);

            return Ok(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("updateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] Guid orderId, [FromQuery] string status)
        {
            if (User == null || !User.Identity.IsAuthenticated) return NotFound();

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            if (orderId == Guid.Empty || string.IsNullOrEmpty(status)) return BadRequest(status);

            var updateOrderStatus = await _orderService.UpdateOrderStatusAndRelatedTable(orderId, status, userId);
            if (!updateOrderStatus) return Ok(new
            {
                status = false,
                message = "Some thing when wrong, can't update order",
            });

            return Ok(new
            {
                status = true,
                message = "Order has been updated sucecssfully!",
            });
        }

        [HttpGet]
        [Route("refund")]
        public async Task<IActionResult> Refund([FromBody] Guid orderId)
        {
            // only refund on settings by admin user.
            // call 3 party to refund order.
            var orderDetail = _orderService.GetOrderDetail(orderId.ToString());
            if (orderDetail == null) return NotFound();

            // only shipment status: returned then allow refund.
            var lastestShipment = _shipmentService.GetShipments().Where(x => x.OrderId == orderId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (lastestShipment != null && lastestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Returned))
            {
                var result = await _paymentService.RefundPaymentForCredit(orderDetail);
                if (result && orderDetail.Order != null)
                {
                    _orderService.UpdateOrder(orderDetail.Order, CommonStatus.OrderStatus.Closed);
                }
                return Ok(result);
            }

            // order can not refund until shipment status change to returned.
            return BadRequest();
        }
    }
}

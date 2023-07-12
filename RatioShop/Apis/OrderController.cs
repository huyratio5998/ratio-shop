using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;

        public OrderController(IOrderService orderService, IShipmentService shipmentService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _shipmentService = shipmentService;
            _paymentService = paymentService;
        }

        [Route("detail")]
        public IActionResult GetOrderDetail([FromBody]Guid orderId)
        {
            // get order
            var orderDetail = _orderService.GetOrderDetail(orderId.ToString());
            // get shipment history of order :
            if (orderDetail == null) return NotFound();

            return Ok(orderDetail);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus([FromHeader] Guid orderId, [FromHeader] string status)
        {
            if (orderId == Guid.Empty || string.IsNullOrEmpty(status)) return BadRequest(status);

            //status: complete / cancel / close /             
            // ++ success => update order status , update shipment status
            // ++ cancel => COD: cancel || Visa: refund. , update shipment status.

            return Ok();
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
            var lastestShipment = _shipmentService.GetShipments().Where(x=>x.OrderId == orderId).OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
            if(lastestShipment != null && lastestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Returned))
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;

        public ShipmentController(IShipmentService shipmentService, IOrderService orderService)
        {
            _shipmentService = shipmentService;
            _orderService = orderService;
        }

        [HttpPost]
        [Route("track")]
        [AllowAnonymous]
        public async Task<IActionResult> ShipmentTracking([FromBody] ShipmentTrackingRequestViewModel request)
        {
            if (request == null) return BadRequest();

            var allowUpdateShipment = true;
            var allowUpdateShipmentOnOrder = true;
            var order = _orderService.GetOrder(request.OrderId.ToString());

            if (order == null) return NotFound();

            // validate shipment status update
            var latestShipment = _shipmentService.GetShipments()
                .Where(x => (x.UpdateStatus != null && (bool)x.UpdateStatus))
                .Where(x => x.OrderId == request.OrderId)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (latestShipment != null
                && !string.IsNullOrEmpty(latestShipment.ShipmentStatus)
                && ((latestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Delivered) && !request.Status.Equals(CommonStatus.ShipmentStatus.Returning))
                    || latestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Returned)
                    || latestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Expired)
                    || latestShipment.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Closed))) allowUpdateShipment = false;

            // update orderStatus compatible with shippingStatus
            var orderStatus = CommonStatus.OrderStatus.Created;
            switch (request.Status)
            {
                case CommonStatus.ShipmentStatus.Returning:
                    orderStatus = CommonStatus.OrderStatus.Canceled;
                    break;
                case CommonStatus.ShipmentStatus.Delivering:
                    orderStatus = CommonStatus.OrderStatus.Delivering;
                    break;
                case CommonStatus.ShipmentStatus.Failure:
                    {
                        var latestShipmentStatusBeingFail = _shipmentService.GetShipments()
                        .Where(x => (x.UpdateStatus != null && (bool)x.UpdateStatus))
                        .Where(x => x.OrderId == request.OrderId)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault(x=>x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Delivering) || x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Returning))
                        .ShipmentStatus;

                        if(latestShipmentStatusBeingFail.Equals(CommonStatus.ShipmentStatus.Delivering)) orderStatus = CommonStatus.OrderStatus.Delivering;
                        else orderStatus = CommonStatus.OrderStatus.Canceled;
                        break;
                    }                    
                case CommonStatus.ShipmentStatus.Canceled:
                    orderStatus = CommonStatus.OrderStatus.Canceled;
                    break;
                case CommonStatus.ShipmentStatus.Delivered:
                    orderStatus = CommonStatus.OrderStatus.Complete;
                    break;
                case CommonStatus.ShipmentStatus.Returned:
                    orderStatus = CommonStatus.OrderStatus.Closed;
                    break;                
                case CommonStatus.ShipmentStatus.Closed:
                    orderStatus = CommonStatus.OrderStatus.Closed;
                    break;                                
                case CommonStatus.ShipmentStatus.Expired:
                    orderStatus = CommonStatus.OrderStatus.Closed;
                    break;
                default:
                    break;
            }

            // add shipment record for each time shipping.
            var shipment = new Data.Models.Shipment()
            {
                Request = JsonConvert.SerializeObject(request),
                ShipmentStatus = request.Status,
                Reasons = request.Reasons,
                Images = request.Images,
                ShipperId = request.ShipperId.ToString(),
                OrderId = request.OrderId,
            };

            if (allowUpdateShipment) shipment.UpdateStatus = true;
            else
            {
                shipment.UpdateStatus = false;
                allowUpdateShipmentOnOrder = false;
            }
            // update shipment to expire while fail to much times.
            var failShipmentsCheck1 = _shipmentService.GetShipments()
                .Where(x => (x.UpdateStatus != null && (bool)x.UpdateStatus))
                .Count(x => x.OrderId == request.OrderId && x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Failure));
            if (failShipmentsCheck1 >= CommonConstant.MaxShippingRetry)
            {
                shipment.UpdateStatus = false;
                allowUpdateShipmentOnOrder = false;
            }
            // still save because need save all request send track api
            await _shipmentService.CreateShipment(shipment);
            order.ShipmentStatus = shipment.ShipmentStatus;

            // update shipment to expire while fail to much times.
            var failShipmentsCheck2 = _shipmentService.GetShipments()
                .Where(x => (x.UpdateStatus != null && (bool)x.UpdateStatus))
                .Count(x => x.OrderId == request.OrderId && x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Failure));

            if (failShipmentsCheck2 == CommonConstant.MaxShippingRetry && allowUpdateShipment)
            {
                request.Status = CommonStatus.ShipmentStatus.Expired;
                request.Reasons = "Over maximum retry shipping time";
                var expireShipment = new Data.Models.Shipment()
                {
                    Request = JsonConvert.SerializeObject(request),
                    ShipmentStatus = request.Status,
                    Reasons = request.Reasons,
                    Images = String.Empty,
                    ShipperId = request.ShipperId.ToString(),
                    OrderId = request.OrderId,
                    UpdateStatus = true
                };

                await _shipmentService.CreateShipment(expireShipment);

                // update order status
                order.ShipmentStatus = expireShipment.ShipmentStatus;
                orderStatus = CommonStatus.OrderStatus.Closed;
                allowUpdateShipmentOnOrder = true;
            }

            if (!allowUpdateShipmentOnOrder) return Ok(new
            {
                allowUpdateShipment = allowUpdateShipment,
                allowUpdateShipmentOnOrder = allowUpdateShipmentOnOrder,
                status = false
            });

            // save
            var saveOrderStatus = _orderService.UpdateOrder(order, orderStatus);

            return Ok(new
            {
                allowUpdateShipment = allowUpdateShipment,
                allowUpdateShipmentOnOrder = allowUpdateShipmentOnOrder,
                status = saveOrderStatus
            });
        }
    }
}

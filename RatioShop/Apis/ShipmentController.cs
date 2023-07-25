using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;
using System.Security.Claims;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IOrderService orderService, IShipmentService shipmentService)
        {
            _orderService = orderService;
            _shipmentService = shipmentService;
        }

        [HttpPost]
        [Route("track")]        
        public async Task<IActionResult> ShipmentTracking([FromBody] ShipmentTrackingRequestViewModel request)
        {
            if (request == null) return BadRequest();

            var result = await _orderService.ShipmentTracking(request);

            return Ok(result);
        }

        [Route("assignShipperToOrder")]
        public IActionResult AssignShipperToOrder(Guid orderId, Guid shipperId)
        {
            if (orderId == Guid.Empty || shipperId == Guid.Empty) return BadRequest();
            // validate user
            if (User == null || !User.Identity.IsAuthenticated) return NotFound();
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            // assign logic
            var latestShipment = _shipmentService.GetShipments().Where(x => x.OrderId == orderId && x.UpdateStatus == true).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            if (latestShipment != null
                && (latestShipment.ShipmentStatus == CommonStatus.ShipmentStatus.Closed
                    || latestShipment.ShipmentStatus == CommonStatus.ShipmentStatus.Expired
                    || latestShipment.ShipmentStatus == CommonStatus.ShipmentStatus.Returned
                )) return Ok(false);

            if (latestShipment == null
                || latestShipment.ShipmentStatus != CommonStatus.ShipmentStatus.Pending
                || (latestShipment.ShipmentStatus == CommonStatus.ShipmentStatus.Pending
                    && !string.IsNullOrEmpty(latestShipment.ShipperId)))
            {
                var newShipment = new Data.Models.Shipment
                {
                    ShipmentStatus = latestShipment == null ? CommonStatus.ShipmentStatus.Pending : latestShipment.ShipmentStatus,
                    SystemMessage = $"Assign shipment. Updated by {userId}",
                    OrderId = orderId,
                    UpdateStatus = true,
                    ShipperId = shipperId.ToString()
                };
                _shipmentService.CreateShipment(newShipment);
                return Ok(true);
            }
            else if (latestShipment.ShipmentStatus == CommonStatus.ShipmentStatus.Pending)
            {
                latestShipment.ShipperId = shipperId.ToString();
                var updateStatus = _shipmentService.UpdateShipment(latestShipment);
                return Ok(updateStatus);
            }

            return Ok(false);
        }

        [Route("shipmentHistoryDetail")]
        public IActionResult ShipmentHistoryDetail(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();

            var shipmentRecord = _shipmentService.GetShipment(id.ToString());

            return Ok(shipmentRecord);
        }
    }
}

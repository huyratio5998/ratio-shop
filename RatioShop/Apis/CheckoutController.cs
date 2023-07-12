using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;

        public CheckoutController(ICartService cartService, IOrderService orderService, IPaymentService paymentService, IShipmentService shipmentService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _shipmentService = shipmentService;
        }

        [HttpPost]
        [Route("cartcheckout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestViewModel request)
        {
            Guid.TryParse(Request.Cookies[CookieKeys.CartId]?.ToString(), out var cartId);
            if (cartId == Guid.Empty || request.PaymentId == Guid.Empty) return BadRequest();

            // validation cart infor
            var cartDetail = _cartService.GetCartDetail(cartId);
            if (cartDetail == null || cartDetail.TotalItems == 0 || string.IsNullOrWhiteSpace(cartDetail.ShippingAddressDetail) || cartDetail.ShippingAddressId == null || cartDetail.ShippingAddressId == 0) return BadRequest();
            // need validate item number again : incase another people add to cart first and that product out of stock.
            // validate
            if (!_cartService.ValidateItemsOnCartWhenCheckout(cartDetail))
                return Ok(new CheckoutResponseViewModel()
                {
                    Status = CommonStatus.Failure,
                    Message = "Some products in cart are out of stock",
                });

            // update store items of related cart for order
            if (!_cartService.UpdateStoreItemsForCart(cartId, ref cartDetail))
                return Ok(new CheckoutResponseViewModel()
                {
                    Status = CommonStatus.Failure,
                    Message = "Some products are not enough in stock",
                });            

            var paymentMethod = _paymentService.GetPaymentAndValidate(request.PaymentId.ToString());
            if (paymentMethod == null) return BadRequest();
            // proceed checkout
            // 
            // save order to DB : status inprogress, shipment infor
            var newOrder = new OrderViewModel()
            {
                Status = CommonStatus.OrderStatus.Created,
                TotalMoney = cartDetail.TotalFinalPrice,
                TotalItems = cartDetail.TotalItems,
                IsRefund = false,
                ShipmentStatus = CommonStatus.ShipmentStatus.Pending,
                ShipmentFee = cartDetail.ShippingFee,
                CartId = cartId,
                PaymentId = request.PaymentId,
            };
            var order = await _orderService.CreateOrder(newOrder);            
            // update product number, stock deduction.                
            var trackingStatus = _cartService.TrackingProductItemByCart(cartDetail, cartId);
            if (!trackingStatus) return Ok(new CheckoutResponseViewModel()
            {
                Status = CommonStatus.Failure,
                Message = "Some products are not enough in stock",
            });
            //
            newOrder.Order = order;
            newOrder.Payment = paymentMethod;
            // + call api 3 party or COD
            var proceedResult = await _paymentService.ProceedPayment(newOrder);
            // get response: 
            // fail: return view checkout and show reasons =>             
            if (!proceedResult)
            {
                _orderService.UpdateOrder(order, CommonStatus.OrderStatus.PendingPayment);
                return Ok(new CheckoutResponseViewModel()
                {
                    Status = CommonStatus.Failure,
                    Message = "Order payment fail",
                });
            }
            // success: return view checkout success, show orderNumber. => update order status:
            if (paymentMethod.Type != Enums.PaymentType.COD)
            {
                _orderService.UpdateOrder(order, CommonStatus.OrderStatus.PaymentRecieved);
            }
            // create shipment record
            await _shipmentService.CreateShipment(new Shipment()
            {
                ShipmentStatus = CommonStatus.ShipmentStatus.Pending,
                Reasons = "Create shipment",
                OrderId = order.Id,
            });

            // update cart status
            _cartService.UpdateCartStatus(cartId.ToString(), CommonStatus.CartStatus.InOrderProcess);
            Response.Cookies.Delete(CookieKeys.CartId);

            return Ok(new CheckoutResponseViewModel()
            {
                Status = CommonStatus.Success,
                Message = "Order created successfully",
                OrderNumber = order.OrderNumber,
            });
        }
    }
}

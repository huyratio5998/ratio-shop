using AutoMapper;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository OrderRepository, IMapper mapper, IPaymentService paymentService, IShipmentService shipmentService, ICartService cartService)
        {
            _orderRepository = OrderRepository;
            _mapper = mapper;
            _paymentService = paymentService;
            _shipmentService = shipmentService;
            _cartService = cartService;
        }

        public Task<Order> CreateOrder(Order Order)
        {
            Order.CreatedDate = DateTime.UtcNow;
            Order.ModifiedDate = DateTime.UtcNow;
            Order.OrderNumber = CommonHelper.BuildOrderNumberByDate("R");
            return _orderRepository.CreateOrder(Order);
        }

        public bool DeleteOrder(string id)
        {
            return _orderRepository.DeleteOrder(id);
        }

        public IEnumerable<Order> GetOrders()
        {
            return _orderRepository.GetOrders();
        }

        public IEnumerable<Order> GetOrders(int pageIndex, int pageSize)
        {
            return _orderRepository.GetOrders();
        }

        public Order? GetOrder(string id)
        {
            return _orderRepository.GetOrder(id);
        }

        public bool UpdateOrder(Order Order)
        {
            Order.ModifiedDate = DateTime.UtcNow;
            return _orderRepository.UpdateOrder(Order);
        }

        public async Task<Order> CreateOrder(OrderViewModel Order)
        {
            try
            {
                var order = _mapper.Map<Order>(Order);
                var result = await CreateOrder(order);
                return result;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public bool UpdateOrder(string orderId, string newStatus)
        {
            var order = GetOrder(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            return UpdateOrder(order);
        }

        public bool UpdateOrder(Order Order, string newStatus)
        {
            Order.Status = newStatus;
            // add logic tracking product while order cancel or refund
            if (newStatus.Equals(CommonStatus.OrderStatus.Closed) || newStatus.Equals(CommonStatus.OrderStatus.Canceled))
            {
                // revert product item being reduce
                var revertTrackingStatus = _cartService.RevertTrackingProductItemByCart(Order.CartId);
                if (!revertTrackingStatus) return false;
            }
            return UpdateOrder(Order);
        }

        public OrderViewModel? GetOrderDetail(string id)
        {
            var order = GetOrder(id);
            if (order == null) return null;

            var cartDetail = _cartService.GetCartDetail(order.CartId);
            if (cartDetail == null) return null;

            var orderDetail = _mapper.Map<OrderViewModel>(order);
            orderDetail.TotalItems = cartDetail.TotalItems;
            orderDetail.Payment = _paymentService.GetPayment(order.PaymentId.ToString());
            orderDetail.Shipments = _shipmentService.GetShipments().Where(x => x.OrderId == order.Id).OrderBy(x => x.CreatedDate).ToList();

            return orderDetail;
        }

        public IEnumerable<OrderViewModel>? GetOrderHistoryByUserId(string userId, int pageIndex = 1, int pageSize = 5)
        {
            if (string.IsNullOrEmpty(userId)) return Enumerable.Empty<OrderViewModel>();
            var orders = GetOrders().AsQueryable()
                .Join(_cartService.GetAllCartByUserId(userId),
                x => x.CartId,
                y => y.Id,
                (x, y) => new { Order = x, Cart = y })                
                .OrderByDescending(x => x.Order.CreatedDate)                
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(x => GetOrderDetail(x.Order.Id.ToString()))
                .Where(x => x != null);

            return orders;
        }

        public int GetTotalOrderByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return 0;

                var orderNumber = GetOrders().AsQueryable()
                .Join(_cartService.GetAllCartByUserId(userId),
                x => x.CartId,
                y => y.Id,
                (x, y) => new { Order = x })
                .Count();

                return orderNumber;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.OrdersViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Data.ViewModels.ShipmentViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
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
            return _orderRepository.GetOrders(pageIndex, pageSize);
        }

        public Order? GetOrder(string id)
        {
            var order = _orderRepository.GetOrder(id);
            order.Payment = _paymentService.GetPayment(order.PaymentId.ToString());
            return order;
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
                var order = Order.Order;
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

        public OrderResponseViewModel? GetOrderDetailResponse(string id, bool getLatestVariantPrice = true, bool isIncludeInActiveDiscount = false)
        {
            var order = GetOrder(id);
            if (order == null) return null;

            var cartDetail = _cartService.GetCartDetail(order.CartId, getLatestVariantPrice, isIncludeInActiveDiscount);
            if (cartDetail == null) return null;

            var orderDetail = _mapper.Map<OrderResponseViewModel>(order);
            orderDetail.CartDetail = cartDetail;
            orderDetail.TotalItems = cartDetail.TotalItems;

            orderDetail.Payment = _mapper.Map<PaymentResponseViewModel>(_paymentService.GetPayment(order.PaymentId.ToString()));
            orderDetail.ShipmentHistory = new ShipmentsResponseViewModel
            {
                Shipments = _mapper.Map<List<ShipmentResponseViewModel>>(
                    _shipmentService.GetShipments()
                    .Where(x => x.OrderId == order.Id && x.UpdateStatus != null && (bool)x.UpdateStatus)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList()
                    )
            };

            return orderDetail;
        }

        public OrderViewModel? GetOrderDetail(string id)
        {
            var order = GetOrder(id);
            if (order == null) return null;

            var cartDetail = _cartService.GetCartDetail(order.CartId);
            if (cartDetail == null) return null;

            var orderDetail = _mapper.Map<OrderViewModel>(order);
            orderDetail.TotalItems = cartDetail.TotalItems;
            orderDetail.Order.Payment = _paymentService.GetPayment(order.PaymentId.ToString());
            orderDetail.Order.Shipments = _shipmentService.GetShipments().Where(x => x.OrderId == order.Id).OrderByDescending(x => x.CreatedDate).ToList();

            return orderDetail;
        }

        public ListOrderViewModel? GetOrderHistoryByUserId(string userId, int pageIndex = 1, int pageSize = 5)
        {
            ListOrderViewModel orderHistories = null;

            if (string.IsNullOrEmpty(userId)) return null;
            var orderSearchQuery = GetOrders().AsQueryable()
                .Join(_cartService.GetAllCartByUserId(userId),
                x => x.CartId,
                y => y.Id,
                (x, y) => new { Order = x, Cart = y })
                .OrderByDescending(x => x.Order.CreatedDate);

            var orderSearchResult = orderSearchQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(x => GetOrderDetail(x.Order.Id.ToString()))
                .Where(x => x != null);

            var totalMatch = orderSearchQuery.Count();
            orderHistories = new ListOrderViewModel
            {
                Orders = orderSearchResult,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = orderSearchQuery.Count(),
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / pageSize),
            };
            return orderHistories;
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

        public ListOrderViewModel? GetOrderHistoryByUserId(string userId, string orderNumber, int pageIndex = 1, int pageSize = 5)
        {
            ListOrderViewModel orderHistories = null;
            var searchText = orderNumber.Trim();

            if (string.IsNullOrEmpty(userId)) return null;
            var orderSearchQuery = GetOrders().AsQueryable()
                .Join(_cartService.GetAllCartByUserId(userId),
                x => x.CartId,
                y => y.Id,
                (x, y) => new { Order = x, Cart = y })
                .Where(x => x.Order.OrderNumber.ToLower().Contains(searchText.ToLower()))
                .OrderByDescending(x => x.Order.CreatedDate);

            var orderSearchResult = orderSearchQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(x => GetOrderDetail(x.Order.Id.ToString()))
                .Where(x => x != null);

            var totalMatch = orderSearchQuery.Count();
            orderHistories = new ListOrderViewModel
            {
                Orders = orderSearchResult,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = orderSearchQuery.Count(),
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / pageSize),
            };
            return orderHistories;
        }

        public ListOrderViewModel GetOrders(BaseSearchRequest args)
        {
            if (args == null) return new ListOrderViewModel();

            var orders = _orderRepository.GetOrders().Include(x=>x.Payment).Select(x=>x);

            orders = BuildOrderFilters(orders, args);

            orders=  orders?.SortedBaseProductsGeneric(args.SortType);
            orders = BuildSortOrder(orders, args);

            var totalCount = orders?.Count() ?? 0;
            orders = orders?.PagingProductsGeneric(args);

            return new ListOrderViewModel
            {
                Orders = _mapper.Map<IEnumerable<OrderViewModel>>(orders),
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
        }

        private IQueryable<Order>? BuildOrderFilters(IQueryable<Order>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            var predicate = PredicateBuilder.True<Order>();
            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.Text:
                        {
                            switch (item.FieldName)
                            {
                                case "":
                                    break;
                                case "OrderNumber":
                                    predicate = predicate.And(x => x.OrderNumber.Contains(item.Value));
                                    break;
                                case "Status":
                                    predicate = predicate.And(x => x.Status.Equals(item.Value));
                                    break;
                                case "ShipmentStatus":
                                    predicate = predicate.And(x => x.ShipmentStatus.Equals(item.Value));
                                    break;
                                case "PaymentType":
                                    {
                                        var getPaymentType = Enum.TryParse(typeof(PaymentType), item.Value, true, out var paymentType);
                                        if (!getPaymentType) continue;
                                        
                                        predicate = predicate.And(x => x.Payment.Type == (PaymentType)paymentType);
                                        break;
                                    }
                                case "IsRefund":
                                    predicate = predicate.And(x => x.IsRefund == bool.Parse(item.Value));
                                    break;
                            }
                            break;
                        }
                }
            }

            return queries.Where(predicate);
        }

        private IQueryable<Order>? BuildSortOrder(IQueryable<Order>? queries, IBaseSort? sort)
        {
            if (queries == null || sort == null) return queries;

            switch (sort.SortType)
            {
                case SortingEnum.HeightoLow:
                    queries = queries.OrderByDescending(x => x.TotalMoney);
                    break;
                case SortingEnum.LowtoHeigh:
                    queries = queries.OrderBy(x => x.TotalMoney);
                    break;
            }

            return queries;
        }
    }
}

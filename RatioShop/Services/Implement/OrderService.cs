using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models;
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

            return UpdateOrder(order, newStatus);
        }

        public bool UpdateOrder(Order order, string newStatus)
        {
            order.Status = newStatus;
            // add logic tracking product while order cancel or refund
            if (newStatus.Equals(CommonStatus.OrderStatus.Closed) || newStatus.Equals(CommonStatus.OrderStatus.Canceled))
            {
                // revert product item being reduce
                var revertTrackingStatus = _cartService.RevertTrackingProductItemByCart(order.CartId);
                if (!revertTrackingStatus) return false;
            }

            if (newStatus.Equals(CommonStatus.OrderStatus.Closed) || newStatus.Equals(CommonStatus.OrderStatus.Complete))
            {
                if (newStatus.Equals(CommonStatus.OrderStatus.Complete))
                {
                    var completeStatus = _cartService.CompleteCartBeingRevertStock(order.CartId);
                    if (!completeStatus) return false;
                }
                _cartService.UpdateCartStatus(order.CartId.ToString(), newStatus);
            }

            return UpdateOrder(order);
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
                    _shipmentService.GetShipments().Include(x=>x.Shipper)
                    .Where(x => x.OrderId == order.Id && x.UpdateStatus != null && (bool)x.UpdateStatus)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList()
                    )
            };

            return orderDetail;
        }

        public OrderResponseViewModel? GetOrderDetailResponse(Order? order, bool getLatestVariantPrice = true, bool isIncludeInActiveDiscount = false)
        {            
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
                    _shipmentService.GetShipments().Include(x => x.Shipper)
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

        public ListOrderViewModel GetOrders(BaseSearchRequest args, List<Guid>? orderIds)
        {         
            if (args == null || orderIds == null || !orderIds.Any()) return new ListOrderViewModel();

            var orders = _orderRepository.GetOrders()
                .Include(x => x.Payment)
                .Select(x => x)
                .Where(x=> orderIds.Any(y=>y == x.Id));

            orders = BuildOrderFilters(orders, args);

            orders = orders?.SortedBaseProductsGeneric(args.SortType);
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

        public async Task<bool> UpdateOrderStatusAndRelatedTable(Guid orderId, string status, string? shipperId = null)
        {
            if (orderId == Guid.Empty || string.IsNullOrEmpty(status)) return false;

            var order = GetOrder(orderId.ToString());

            if (order == null) return false;
            if (order.Status.Equals(status)) return true;

            var shipmentUpdate = new Shipment()
            {
                Reasons = "Trigger update order status by admin",
                OrderId = orderId,
                UpdateStatus = true,
                ShipperId = shipperId
            };

            switch (status)
            {
                case CommonStatus.OrderStatus.Complete:
                    shipmentUpdate.ShipmentStatus = CommonStatus.ShipmentStatus.Delivered;
                    break;
                case CommonStatus.OrderStatus.Canceled:
                    shipmentUpdate.ShipmentStatus = CommonStatus.ShipmentStatus.Canceled;
                    break;
                case CommonStatus.OrderStatus.Closed:
                    shipmentUpdate.ShipmentStatus = CommonStatus.ShipmentStatus.Closed;
                    break;
            }

            if (!string.IsNullOrEmpty(shipmentUpdate.ShipmentStatus)) order.ShipmentStatus = shipmentUpdate.ShipmentStatus;
            var updateOrderStatus = UpdateOrder(order, status);

            if(updateOrderStatus && !string.IsNullOrEmpty(shipmentUpdate.ShipmentStatus)) await _shipmentService.CreateShipment(shipmentUpdate);

            return updateOrderStatus;
        }

        public async Task<ShipmentTrackingResponse> ShipmentTracking(ShipmentTrackingRequestViewModel request)
        {
            if (request == null) return new ShipmentTrackingResponse();

            var allowUpdateShipment = true;
            var allowUpdateShipmentOnOrder = true;
            var order = GetOrder(request.OrderId.ToString());

            if (order == null) return new ShipmentTrackingResponse();

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
                        .FirstOrDefault(x => x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Delivering) || x.ShipmentStatus.Equals(CommonStatus.ShipmentStatus.Returning))?
                        .ShipmentStatus;

                        if (!string.IsNullOrEmpty(latestShipmentStatusBeingFail) && latestShipmentStatusBeingFail.Equals(CommonStatus.ShipmentStatus.Delivering)) orderStatus = CommonStatus.OrderStatus.Delivering;
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
            var shipment = new Shipment()
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

            if (!allowUpdateShipmentOnOrder) return new ShipmentTrackingResponse
            {
                AllowUpdateShipment = allowUpdateShipment,
                AllowUpdateShipmentOnOrder = allowUpdateShipmentOnOrder,
                Status = false
            };

            // save
            var saveOrderStatus = UpdateOrder(order, orderStatus);

            return new ShipmentTrackingResponse
            {
                AllowUpdateShipment = allowUpdateShipment,
                AllowUpdateShipmentOnOrder = allowUpdateShipmentOnOrder,
                Status = saveOrderStatus
            };
        }
    }
}

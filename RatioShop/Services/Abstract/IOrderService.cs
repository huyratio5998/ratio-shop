using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.OrdersViewModel;

namespace RatioShop.Services.Abstract
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrders();
        IEnumerable<Order> GetOrders(int pageIndex, int pageSize);
        Order? GetOrder(string id);
        Task<Order> CreateOrder(Order Order);
        Task<Order> CreateOrder(OrderViewModel Order);
        bool UpdateOrder(Order Order);
        bool DeleteOrder(string id);

        // Business logics
        bool UpdateOrder(Order order, string newStatus);
        bool UpdateOrder(string orderId, string newStatus);
        OrderViewModel? GetOrderDetail(string id);
        IEnumerable<OrderViewModel>? GetOrderHistoryByUserId(string userId, int pageIndex = 1, int pageSize = 5);
        int GetTotalOrderByUserId(string userId);
        OrderResponseViewModel? GetOrderDetailResponse(string id);
    }
}

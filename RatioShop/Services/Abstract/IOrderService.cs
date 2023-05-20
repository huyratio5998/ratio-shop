using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrders();
        Order? GetOrder(int id);
        Task<Order> CreateOrder(Order Order);
        bool UpdateOrder(Order Order);
        bool DeleteOrder(int id);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        IEnumerable<Order> GetOrders(int pageIndex, int pageSize);
        Order? GetOrder(string id);
        Task<Order> CreateOrder(Order Order);
        bool UpdateOrder(Order Order);        
        bool DeleteOrder(string id);
    }
}

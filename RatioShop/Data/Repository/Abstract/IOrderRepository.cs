using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        Order? GetOrder(int id);
        Task<Order> CreateOrder(Order Order);
        bool UpdateOrder(Order Order);        
        bool DeleteOrder(int id);
    }
}

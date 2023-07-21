using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrders();
        IQueryable<Order> GetOrders(int pageIndex, int pageSize);
        Order? GetOrder(string id);
        Task<Order> CreateOrder(Order Order);
        bool UpdateOrder(Order Order);        
        bool DeleteOrder(string id);
    }
}

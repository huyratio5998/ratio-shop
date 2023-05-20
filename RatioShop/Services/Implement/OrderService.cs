using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _OrderRepository;

        public OrderService(IOrderRepository OrderRepository)
        {
            _OrderRepository = OrderRepository;
        }

        public Task<Order> CreateOrder(Order Order)
        {
            Order.CreatedDate = DateTime.UtcNow;
            Order.ModifiedDate = DateTime.UtcNow;
            return _OrderRepository.CreateOrder(Order);
        }

        public bool DeleteOrder(int id)
        {
            return _OrderRepository.DeleteOrder(id);
        }

        public IEnumerable<Order> GetOrders()
        {
            return _OrderRepository.GetOrders();
        }

        public Order? GetOrder(int id)
        {
            return _OrderRepository.GetOrder(id);
        }

        public bool UpdateOrder(Order Order)
        {
            Order.ModifiedDate = DateTime.UtcNow;
            return _OrderRepository.UpdateOrder(Order);
        }
    }
}

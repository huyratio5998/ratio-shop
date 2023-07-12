using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class OrderRepository : BaseProductRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order> CreateOrder(Order Order)
        {
            return await Create(Order);
        }

        public bool DeleteOrder(string id)
        {
            return Delete(id);
        }

        public IEnumerable<Order> GetOrders()
        {
            return GetAll();
        }

        public IEnumerable<Order> GetOrders(int pageIndex, int pageSize)
        {
            return GetAll(pageIndex, pageSize);
        }

        public Order? GetOrder(string id)
        {
            return GetById(id);
        }

        public bool UpdateOrder(Order Order)
        {
            return Update(Order);
        }
    }
}

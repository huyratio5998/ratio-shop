using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class PaymentRepository : BaseProductRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment> CreatePayment(Payment Payment)
        {
            return await Create(Payment);
        }

        public bool DeletePayment(string id)
        {
            return Delete(id);
        }

        public IEnumerable<Payment> GetPayments()
        {
            return GetAll();
        }

        public Payment? GetPayment(string id)
        {
            return GetById(id);
        }

        public bool UpdatePayment(Payment Payment)
        {
            return Update(Payment);
        }
    }
}

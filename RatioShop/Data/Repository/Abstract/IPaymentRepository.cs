using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IPaymentRepository
    {
        IEnumerable<Payment> GetPayments();
        Payment? GetPayment(int id);
        Task<Payment> CreatePayment(Payment Payment);
        bool UpdatePayment(Payment Payment);        
        bool DeletePayment(int id);
    }
}

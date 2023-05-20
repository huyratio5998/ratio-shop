using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IPaymentService
    {
        IEnumerable<Payment> GetPayments();
        Payment? GetPayment(int id);
        Task<Payment> CreatePayment(Payment Payment);
        bool UpdatePayment(Payment Payment);
        bool DeletePayment(int id);
    }
}

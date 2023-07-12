using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;

namespace RatioShop.Services.Abstract
{
    public interface IPaymentService
    {
        IEnumerable<Payment> GetPayments();
        Payment? GetPayment(string id);
        Task<Payment> CreatePayment(Payment Payment);
        bool UpdatePayment(Payment Payment);
        bool DeletePayment(string id);

        // business logics
        Payment? GetPaymentAndValidate(string id);
        Task<bool> ProceedPayment(OrderViewModel order);
        Task<bool> RefundPaymentForCredit(OrderViewModel order);
    }
}

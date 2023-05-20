using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _PaymentRepository;

        public PaymentService(IPaymentRepository PaymentRepository)
        {
            _PaymentRepository = PaymentRepository;
        }

        public Task<Payment> CreatePayment(Payment Payment)
        {
            Payment.CreatedDate = DateTime.UtcNow;
            Payment.ModifiedDate = DateTime.UtcNow;
            return _PaymentRepository.CreatePayment(Payment);
        }

        public bool DeletePayment(int id)
        {
            return _PaymentRepository.DeletePayment(id);
        }

        public IEnumerable<Payment> GetPayments()
        {
            return _PaymentRepository.GetPayments();
        }

        public Payment? GetPayment(int id)
        {
            return _PaymentRepository.GetPayment(id);
        }

        public bool UpdatePayment(Payment Payment)
        {
            Payment.ModifiedDate = DateTime.UtcNow;
            return _PaymentRepository.UpdatePayment(Payment);
        }
    }
}

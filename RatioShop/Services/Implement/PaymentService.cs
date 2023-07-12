using RatioShop.Constants;
using RatioShop.Data.HttpClientFactoryClientType.Payments;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Enums;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _PaymentRepository;        
        private readonly PaypalClient _paypalClient;

        public PaymentService(IPaymentRepository PaymentRepository, PaypalClient paypalClient)
        {
            _PaymentRepository = PaymentRepository;                        
            _paypalClient = paypalClient;
        }

        public Task<Payment> CreatePayment(Payment Payment)
        {
            Payment.CreatedDate = DateTime.UtcNow;
            Payment.ModifiedDate = DateTime.UtcNow;
            return _PaymentRepository.CreatePayment(Payment);
        }

        public bool DeletePayment(string id)
        {
            return _PaymentRepository.DeletePayment(id);
        }

        public IEnumerable<Payment> GetPayments()
        {
            return _PaymentRepository.GetPayments();
        }

        public Payment? GetPayment(string id)
        {
            return _PaymentRepository.GetPayment(id);
        }

        public bool UpdatePayment(Payment Payment)
        {
            Payment.ModifiedDate = DateTime.UtcNow;
            return _PaymentRepository.UpdatePayment(Payment);
        }

        public async Task<bool> ProceedPayment(OrderViewModel order)
        {
            var paymentMethod = order.Payment?.Type;
            if (paymentMethod == null) return false;
            if (paymentMethod == PaymentType.COD) return true;

            return await PaymentForCredit(order);            
        }

        private async Task<bool> PaymentForCredit(OrderViewModel order)
        {            
            try
            {
                var response = await _paypalClient.ProceedPayment(order);
                if (response == null) return false;               
                return true;

            }
            catch (Exception)
            {
                return false;
            }                        
        }

        public async Task<bool> RefundPaymentForCredit(OrderViewModel order)
        {
            string url = "";
            try
            {
                var response = await _paypalClient.ProceedRefundPayment(order);
                if (response == null) return false;                
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public Payment? GetPaymentAndValidate(string id)
        {
            var payment = GetPayment(id);
            if(payment == null || !payment.IsActive) return null;
            return payment;
        }
    }
}

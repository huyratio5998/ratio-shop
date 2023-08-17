using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository DiscountRepository)
        {
            _discountRepository = DiscountRepository;
        }

        public Task<Discount> CreateDiscount(Discount Discount)
        {
            Discount.CreatedDate = DateTime.UtcNow;
            Discount.ModifiedDate = DateTime.UtcNow;
            return _discountRepository.CreateDiscount(Discount);
        }

        public bool DeleteDiscount(int id)
        {
            return _discountRepository.DeleteDiscount(id);
        }

        public IEnumerable<Discount> GetDiscounts()
        {
            return _discountRepository.GetDiscounts();
        }

        public Discount? GetDiscount(int id)
        {
            return _discountRepository.GetDiscount(id);
        }

        public bool UpdateDiscount(Discount Discount)
        {
            Discount.ModifiedDate = DateTime.UtcNow;
            return _discountRepository.UpdateDiscount(Discount);
        }

        public Discount? GetDiscountByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            return _discountRepository.GetDiscounts().ToList().FirstOrDefault(x => x.Code.Equals(code) && x.Status.Equals(CommonStatus.Discount.Active) && !x.IsDelete);
        }

        public bool TemporaryDeleteDiscount(int id)
        {
            var discount = GetDiscount(id);
            if(discount == null) return false;

            discount.IsDelete = true;
            discount.Status = CommonStatus.Discount.InActive;
            return UpdateDiscount(discount);
        }
    }
}

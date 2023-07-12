using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class DiscountRepository : BaseEntityRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Discount> CreateDiscount(Discount Discount)
        {
            return await Create(Discount);
        }

        public bool DeleteDiscount(int id)
        {
            return Delete(id);
        }

        public IQueryable<Discount> GetDiscounts()
        {
            return GetAll();
        }

        public Discount? GetDiscount(int id)
        {
            return GetById(id);
        }

        public bool UpdateDiscount(Discount Discount)
        {
            return Update(Discount);
        }
    }
}

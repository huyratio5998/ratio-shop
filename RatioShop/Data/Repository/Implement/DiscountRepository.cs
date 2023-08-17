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
            try
            {
                _context.Entry(Discount).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Entry(Discount).Property(x => x.Value).IsModified = false;
                _context.Entry(Discount).Property(x => x.Code).IsModified = false;
                _context.Entry(Discount).Property(x => x.DiscountType).IsModified = false;                
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using System.Linq.Expressions;

namespace RatioShop.Data.Repository.Implement
{
    public class CartDiscountRepository : BaseEntityRepository<CartDiscount>, ICartDiscountRepository
    {
        public CartDiscountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CartDiscount> CreateCartDiscount(CartDiscount CartDiscount)
        {
            return await Create(CartDiscount);
        }

        public bool DeleteCartDiscount(int id)
        {
            return Delete(id);
        }

        public IQueryable<CartDiscount> GetCartDiscounts()
        {
            return GetAll();
        }

        public CartDiscount? GetCartDiscount(int id)
        {
            return GetById(id);
        }

        public bool UpdateCartDiscount(CartDiscount CartDiscount)
        {
            return Update(CartDiscount);
        }

        CartDiscount? ICartDiscountRepository.Find(Expression<Func<CartDiscount, bool>> predicate)
        {
            return Find(predicate).FirstOrDefault();
        }
    }
}

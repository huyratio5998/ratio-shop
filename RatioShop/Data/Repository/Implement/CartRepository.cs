using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class CartRepository : BaseProductRepository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cart> CreateCart(Cart Cart)
        {
            return await Create(Cart);
        }

        public bool DeleteCart(string id)
        {
            return Delete(id);
        }

        public IQueryable<Cart> GetCarts()
        {
            return GetAll();
        }

        public Cart? GetCart(string id)
        {
            return GetById(id);
        }

        public bool UpdateCart(Cart Cart)
        {
            return Update(Cart);
        }
    }
}

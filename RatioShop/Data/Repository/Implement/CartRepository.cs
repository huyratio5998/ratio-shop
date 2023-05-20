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

        public bool DeleteCart(int id)
        {
            return Delete(id);
        }

        public IEnumerable<Cart> GetCarts()
        {
            return GetAll();
        }

        public Cart? GetCart(int id)
        {
            return GetById(id);
        }

        public bool UpdateCart(Cart Cart)
        {
            return Update(Cart);
        }
    }
}

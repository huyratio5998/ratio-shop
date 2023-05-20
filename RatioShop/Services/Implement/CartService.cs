using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _CartRepository;

        public CartService(ICartRepository CartRepository)
        {
            _CartRepository = CartRepository;
        }

        public Task<Cart> CreateCart(Cart Cart)
        {
            Cart.CreatedDate = DateTime.UtcNow;
            Cart.ModifiedDate = DateTime.UtcNow;
            return _CartRepository.CreateCart(Cart);
        }

        public bool DeleteCart(int id)
        {
            return _CartRepository.DeleteCart(id);
        }

        public IEnumerable<Cart> GetCarts()
        {
            return _CartRepository.GetCarts();
        }

        public Cart? GetCart(int id)
        {
            return _CartRepository.GetCart(id);
        }

        public bool UpdateCart(Cart Cart)
        {
            Cart.ModifiedDate = DateTime.UtcNow;
            return _CartRepository.UpdateCart(Cart);
        }
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface ICartService
    {
        IEnumerable<Cart> GetCarts();
        Cart? GetCart(int id);
        Task<Cart> CreateCart(Cart Cart);
        bool UpdateCart(Cart Cart);
        bool DeleteCart(int id);
    }
}

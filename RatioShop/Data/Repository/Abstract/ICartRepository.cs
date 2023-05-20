using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICartRepository
    {
        IEnumerable<Cart> GetCarts();
        Cart? GetCart(int id);
        Task<Cart> CreateCart(Cart Cart);
        bool UpdateCart(Cart Cart);        
        bool DeleteCart(int id);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICartRepository
    {
        IQueryable<Cart> GetCarts();
        Cart? GetCart(string id);
        Task<Cart> CreateCart(Cart Cart);
        bool UpdateCart(Cart Cart);        
        bool DeleteCart(string id);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IShopUserRepository
    {
        IEnumerable<ShopUser> GetShopUsers();
        ShopUser? GetShopUser(string id);
        Task<ShopUser> CreateShopUser(ShopUser ShopUser);
        bool UpdateShopUser(ShopUser ShopUser);        
        bool DeleteShopUser(string id);
    }
}

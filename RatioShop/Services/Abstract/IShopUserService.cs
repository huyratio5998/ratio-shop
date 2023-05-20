using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IShopUserService
    {
        IEnumerable<ShopUser> GetShopUsers();
        ShopUser? GetShopUser(string id);
        Task<ShopUser> CreateShopUser(ShopUser ShopUser);
        bool UpdateShopUser(ShopUser ShopUser);
        bool DeleteShopUser(string id);
    }
}

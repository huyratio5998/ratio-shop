using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ShopUserService : IShopUserService
    {
        private readonly IShopUserRepository _ShopUserRepository;

        public ShopUserService(IShopUserRepository ShopUserRepository)
        {
            _ShopUserRepository = ShopUserRepository;
        }

        public Task<ShopUser> CreateShopUser(ShopUser ShopUser)
        {
            return _ShopUserRepository.CreateShopUser(ShopUser);
        }

        public bool DeleteShopUser(string id)
        {
            return _ShopUserRepository.DeleteShopUser(id);
        }

        public IEnumerable<ShopUser> GetShopUsers()
        {
            return _ShopUserRepository.GetShopUsers();
        }

        public ShopUser? GetShopUser(string id)
        {
            return _ShopUserRepository.GetShopUser(id);
        }

        public bool UpdateShopUser(ShopUser ShopUser)
        {
            return _ShopUserRepository.UpdateShopUser(ShopUser);
        }
    }
}

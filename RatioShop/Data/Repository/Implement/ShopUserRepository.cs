using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ShopUserRepository : IShopUserRepository
    {
        protected ApplicationDbContext _context;
        public ShopUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShopUser> CreateShopUser(ShopUser ShopUser)
        {
            await _context.AddAsync(ShopUser);
            _context.SaveChanges();
            return ShopUser;
        }

        public bool DeleteShopUser(string id)
        {
            var entity = GetShopUser(id);
            if (entity == null) return false;

            _context.Set<ShopUser>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public ShopUser? GetShopUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            return _context.Set<ShopUser>().AsNoTracking().FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ShopUser> GetShopUsers()
        {
            return _context.Set<ShopUser>().AsNoTracking();
        }

        public IEnumerable<ShopUser> GetShopUsers(int pageIndex, int pageSize)
        {
            return _context.Set<ShopUser>()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        public bool UpdateShopUser(ShopUser ShopUser)
        {
            try
            {
                _context.Set<ShopUser>().Update(ShopUser);
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

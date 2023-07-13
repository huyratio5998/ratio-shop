using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Helpers;

namespace RatioShop.Data.Repository
{
    public class BaseProductRepository<T> : BaseRepository<T> where T : BaseProduct
    {
        public BaseProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override bool Delete(string id)
        {
            var entity = GetById(id);
            if(entity == null) return false;

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override T? GetById(string id)
        {
            if(string.IsNullOrEmpty(id)) return null;
            
            var result = _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id.ToString().ToLower().Equals(id));
            if (result == null) return null;

            result.CreatedDate = result.CreatedDate.GetCorrectUTC();
            result.ModifiedDate = result.ModifiedDate.GetCorrectUTC();
            return result;
        }

        public override T? GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}

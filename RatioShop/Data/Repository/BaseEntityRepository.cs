using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Helpers;

namespace RatioShop.Data.Repository
{
    public class BaseEntityRepository<T> : BaseRepository<T> where T : BaseEntity
    {
        public BaseEntityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override bool Delete(string id)
        {
            var entity = GetById(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public override bool Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public override T? GetById(string id, bool isTracking = false)
        {
            if (string.IsNullOrEmpty(id)) return null;
            T? result;
            
            if (isTracking) result = _context.Set<T>().FirstOrDefault(x => x.Id.ToString().ToLower().Equals(id.ToLower()));
            else result = _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id.ToString().ToLower().Equals(id.ToLower()));

            if (result == null) return null;

            result.CreatedDate = result.CreatedDate.GetCorrectUTC();
            result.ModifiedDate = result.ModifiedDate.GetCorrectUTC();            
            return result;
        }

        public override T? GetById(int id, bool isTracking = false)
        {
            if (id == 0) return null;            
            T? result = null;

            if(isTracking) result = _context.Set<T>().FirstOrDefault(x => x.Id == id);
            else result = _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (result == null) return null;

            result.CreatedDate = result.CreatedDate.GetCorrectUTC();
            result.ModifiedDate = result.ModifiedDate.GetCorrectUTC();            
            return result;
        }
    }
}

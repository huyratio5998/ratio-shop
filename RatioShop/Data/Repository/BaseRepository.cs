using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RatioShop.Data.Repository
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T> Create(T entity)
        {
            await _context.AddAsync(entity);
            _context.SaveChanges();
            return entity;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable().AsNoTracking();
        }

        public IQueryable<T> GetAll(int pageIndex, int pageSize)
        {
            return _context.Set<T>()
                .AsQueryable()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        public bool Update(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public abstract bool Delete(string id);
        public abstract bool Delete(int id);
        public abstract T? GetById(int id);
        public abstract T? GetById(string id);

        public IQueryable<T> Find(Expression<Func<T, bool>> condition)
        {
            return GetAll().Where(condition);
        }
    }
}

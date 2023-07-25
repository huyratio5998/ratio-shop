using System.Linq.Expressions;

namespace RatioShop.Data.Repository
{
    public interface IBaseRepository<T> where T : class
    {        
        public IQueryable<T> GetAll(bool isTracking = false);
        public IQueryable<T> GetAll(int pageIndex, int pageSize);
        public Task<T> Create(T entity);
        public bool Update(T entity, bool isTracking = false);        
        public IQueryable<T> Find(Expression<Func<T, bool>> condition);
    }
}

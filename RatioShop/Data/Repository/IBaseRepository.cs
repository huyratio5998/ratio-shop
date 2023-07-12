using System.Linq.Expressions;

namespace RatioShop.Data.Repository
{
    public interface IBaseRepository<T> where T : class
    {        
        public IQueryable<T> GetAll();
        public IQueryable<T> GetAll(int pageIndex, int pageSize);
        public Task<T> Create(T entity);
        public bool Update(T entity);        
        public IQueryable<T> Find(Expression<Func<T, bool>> condition);
    }
}

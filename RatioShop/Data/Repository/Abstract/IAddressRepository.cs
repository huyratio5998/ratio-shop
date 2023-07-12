using RatioShop.Data.Models;
using System.Linq.Expressions;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IAddressRepository
    {
        IQueryable<Address> GetAddresses();
        Address? GetAddress(int id);
        Task<Address> CreateAddress(Address Address);
        bool UpdateAddress(Address Address);        
        bool DeleteAddress(int id);
        Address? Find(Expression<Func<Address,bool>> predicate);
    }
}

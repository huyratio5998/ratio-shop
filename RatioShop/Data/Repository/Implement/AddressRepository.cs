using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using System.Linq.Expressions;

namespace RatioShop.Data.Repository.Implement
{
    public class AddressRepository : BaseEntityRepository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Address> CreateAddress(Address Address)
        {
            return await Create(Address);
        }

        public bool DeleteAddress(int id)
        {
            return Delete(id);
        }

        public IQueryable<Address> GetAddresses()
        {
            return GetAll();
        }

        public Address? GetAddress(int id)
        {
            return GetById(id);
        }

        public bool UpdateAddress(Address Address)
        {
            return Update(Address);
        }

        Address? IAddressRepository.Find(Expression<Func<Address, bool>> predicate)
        {
            return Find(predicate).FirstOrDefault();
        }
    }
}

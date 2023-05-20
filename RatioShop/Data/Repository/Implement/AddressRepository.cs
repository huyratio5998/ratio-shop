using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

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

        public IEnumerable<Address> GetAddresss()
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
    }
}

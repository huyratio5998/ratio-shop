using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _AddressRepository;

        public AddressService(IAddressRepository AddressRepository)
        {
            _AddressRepository = AddressRepository;
        }

        public Task<Address> CreateAddress(Address Address)
        {
            Address.CreatedDate = DateTime.UtcNow;
            Address.ModifiedDate = DateTime.UtcNow;
            return _AddressRepository.CreateAddress(Address);
        }

        public bool DeleteAddress(int id)
        {
            return _AddressRepository.DeleteAddress(id);
        }

        public IEnumerable<Address> GetAddresss()
        {
            return _AddressRepository.GetAddresss();
        }

        public Address? GetAddress(int id)
        {
            return _AddressRepository.GetAddress(id);
        }

        public bool UpdateAddress(Address Address)
        {
            Address.ModifiedDate = DateTime.UtcNow;
            return _AddressRepository.UpdateAddress(Address);
        }
    }
}

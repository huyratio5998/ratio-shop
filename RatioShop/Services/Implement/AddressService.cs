using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;
using System.Linq.Expressions;

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

        public IEnumerable<Address> GetAddresses()
        {
            return _AddressRepository.GetAddresses().AsQueryable().OrderBy(x=>x.Address1);
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

        public IEnumerable<Address> GetAddressesByValueOfType(string type, string value)
        {
            var addressType = new string[] { "Address1", "Address2", "Address3", "Address4", "Address5" };
            if(string.IsNullOrEmpty(type) || !addressType.Contains(type.ToString())) return Enumerable.Empty<Address>();

            var param = Expression.Parameter(typeof(Address), "x");            
            var lambda = Expression.Lambda<Func<Address, bool>>(Expression.Equal(Expression.Property(param, type), Expression.Constant(value)),param);
            var addresses = _AddressRepository.GetAddresses().Where(lambda.Compile()).Where(x=>x.IsActive).OrderBy(x=>x.Address1);

            return addresses;
        }

        public Address? GetAddressByCityAndDistrict(string city, string district)
        {
            if(string.IsNullOrEmpty(city) || string.IsNullOrEmpty(district)) return null;

            var shippinFee = _AddressRepository.Find(x => x.Address1.Equals(city) && x.Address2.Equals(district));
            return shippinFee;
        }

        public IEnumerable<string> GetAddressesByType(string type)
        {
            var addressType = new string[] { "Address1", "Address2", "Address3", "Address4", "Address5" };
            if (string.IsNullOrEmpty(type) || !addressType.Contains(type.ToString())) return Enumerable.Empty<string>();

            var param = Expression.Parameter(typeof(Address), "x");
            var lambda = Expression.Lambda<Func<Address, string>>(Expression.PropertyOrField(param, type), param);
            var addresses = _AddressRepository.GetAddresses().Where(x => x.IsActive).Select(lambda.Compile()).Distinct().OrderBy(x=> x);

            return addresses;
        }
    }
}

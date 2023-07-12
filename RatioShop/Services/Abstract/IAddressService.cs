using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IAddressService
    {
        IEnumerable<Address> GetAddresses();
        Address? GetAddress(int id);
        Task<Address> CreateAddress(Address Address);
        bool UpdateAddress(Address Address);
        bool DeleteAddress(int id);

        IEnumerable<Address> GetAddressesByValueOfType(string type, string value);
        IEnumerable<string> GetAddressesByType(string type);
        Address? GetAddressByCityAndDistrict(string city, string district);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IAddressService
    {
        IEnumerable<Address> GetAddresss();
        Address? GetAddress(int id);
        Task<Address> CreateAddress(Address Address);
        bool UpdateAddress(Address Address);
        bool DeleteAddress(int id);
    }
}

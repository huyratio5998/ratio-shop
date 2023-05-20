using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IAddressRepository
    {
        IEnumerable<Address> GetAddresss();
        Address? GetAddress(int id);
        Task<Address> CreateAddress(Address Address);
        bool UpdateAddress(Address Address);        
        bool DeleteAddress(int id);
    }
}

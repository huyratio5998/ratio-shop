using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IPackageRepository : IBaseRepository<Package>
    {
        IQueryable<Package> GetPackages();
        IQueryable<Package> GetPackages(int pageIndex, int pageSize);
        Package? GetPackage(string id);
        Task<Package> CreatePackage(Package Package);
        bool UpdatePackage(Package Package);
        bool DeletePackage(string id);
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class PackageRepository : BaseProductRepository<Package>, IPackageRepository
    {
        public PackageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Package> CreatePackage(Package Package)
        {
            return await Create(Package);
        }

        public bool DeletePackage(string id)
        {
            return Delete(id);
        }

        public IQueryable<Package> GetPackages()
        {            
            return GetAll();
        }

        public IQueryable<Package> GetPackages(int pageIndex, int pageSize)
        {
            return GetAll(pageIndex, pageSize);
        }

        public Package? GetPackage(string id)
        {
            return GetById(id);
        }

        public bool UpdatePackage(Package Package)
        {
            return Update(Package);
        }
    }
}

using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class CatalogRepository : BaseEntityRepository<Catalog>, ICatalogRepository
    {
        public CatalogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Catalog> CreateCatalog(Catalog Catalog)
        {
            return await Create(Catalog);
        }

        public bool DeleteCatalog(int id)
        {
            return Delete(id);
        }

        public IEnumerable<Catalog> GetCatalogs()
        {
            return GetAll();
        }

        public Catalog? GetCatalog(int id)
        {
            return GetById(id);
        }

        public bool UpdateCatalog(Catalog Catalog)
        {
            return Update(Catalog);
        }
    }
}

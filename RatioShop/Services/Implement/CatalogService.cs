using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _CatalogRepository;

        public CatalogService(ICatalogRepository CatalogRepository)
        {
            _CatalogRepository = CatalogRepository;
        }

        public Task<Catalog> CreateCatalog(Catalog Catalog)
        {
            Catalog.CreatedDate = DateTime.UtcNow;
            Catalog.ModifiedDate = DateTime.UtcNow;
            return _CatalogRepository.CreateCatalog(Catalog);
        }

        public bool DeleteCatalog(int id)
        {
            return _CatalogRepository.DeleteCatalog(id);
        }

        public IEnumerable<Catalog> GetCatalogs()
        {
            return _CatalogRepository.GetCatalogs();
        }

        public Catalog? GetCatalog(int id)
        {
            return _CatalogRepository.GetCatalog(id);
        }

        public bool UpdateCatalog(Catalog Catalog)
        {
            Catalog.ModifiedDate = DateTime.UtcNow;
            return _CatalogRepository.UpdateCatalog(Catalog);
        }
    }
}

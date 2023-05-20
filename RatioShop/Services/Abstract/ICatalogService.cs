using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface ICatalogService
    {
        IEnumerable<Catalog> GetCatalogs();
        Catalog? GetCatalog(int id);
        Task<Catalog> CreateCatalog(Catalog Catalog);
        bool UpdateCatalog(Catalog Catalog);
        bool DeleteCatalog(int id);
    }
}

using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface ICatalogRepository
    {
        IEnumerable<Catalog> GetCatalogs();
        Catalog? GetCatalog(int id);
        Task<Catalog> CreateCatalog(Catalog Catalog);
        bool UpdateCatalog(Catalog Catalog);        
        bool DeleteCatalog(int id);
    }
}

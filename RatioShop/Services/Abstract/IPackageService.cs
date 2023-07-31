using RatioShop.Areas.Admin.Models;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;

namespace RatioShop.Services.Abstract
{
    public interface IPackageService
    {
        IEnumerable<Package> GetPackages();
        Package? GetPackage(string id);
        Task<Package> CreatePackage(Package Package);
        bool UpdatePackage(Package Package);
        bool DeletePackage(string id);

        ListPackageViewModel GetPackages(BaseSearchRequest request);
        PackageViewModel? GetPackageViewModel(Guid id);
        ProductVariantPackage? GetPackageItem(Guid id, Guid packageId);
        Task<bool> CreatePackageItem(Guid id, Guid packageId, int itemNumber);
        bool UpdatePackageItem(Guid id, Guid packageId, int itemNumber);
        bool UpdatePackageItem(ProductVariantPackage productVariantPackage);
        bool DeletePackageItem(Guid id, Guid packageId);

    }
}

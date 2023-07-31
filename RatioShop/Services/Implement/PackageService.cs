using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RatioShop.Areas.Admin.Models;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IProductVariantPackageService _productVariantPackageService;
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;

        public PackageService(IPackageRepository PackageRepository, IMapper mapper, IProductVariantPackageService productVariantPackageService, IProductVariantService productVariantService)
        {
            _packageRepository = PackageRepository;
            _mapper = mapper;
            _productVariantPackageService = productVariantPackageService;
            _productVariantService = productVariantService;
        }

        public Task<Package> CreatePackage(Package Package)
        {
            Package.CreatedDate = DateTime.UtcNow;
            Package.ModifiedDate = DateTime.UtcNow;
            return _packageRepository.CreatePackage(Package);
        }

        public bool DeletePackage(string id)
        {
            return _packageRepository.DeletePackage(id);
        }

        public IEnumerable<Package> GetPackages()
        {
            return _packageRepository.GetPackages();
        }

        public Package? GetPackage(string id)
        {
            return _packageRepository.GetPackage(id);
        }

        public bool UpdatePackage(Package Package)
        {
            Package.ModifiedDate = DateTime.UtcNow;
            return _packageRepository.UpdatePackage(Package);
        }

        public ListPackageViewModel GetPackages(BaseSearchRequest args)
        {
            if (args == null) return new ListPackageViewModel();

            var packages = _packageRepository.GetPackages();

            packages = BuildPackageFilters(packages, args);

            packages = packages?.SortedBaseProductsGeneric(args.SortType);
            packages = BuildSortPackage(packages, args);

            var totalCount = packages?.Count() ?? 0;
            packages = packages?.PagingProductsGeneric(args);

            return new ListPackageViewModel
            {
                Packages = _mapper.Map<List<PackageViewModel>>(packages),
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
        }

        private IQueryable<Package>? BuildPackageFilters(IQueryable<Package>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            var predicate = PredicateBuilder.True<Package>();
            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.Text:
                        {
                            switch (item.FieldName)
                            {
                                case "":
                                    break;
                                case "Code":
                                    predicate = predicate.And(x => x.Code.Contains(item.Value));
                                    break;
                                case "Name":
                                    predicate = predicate.And(x => x.Name.Contains(item.Value) || x.ProductFriendlyName.Contains(item.Value));
                                    break;
                            }
                            break;
                        }
                }
            }

            return queries.Where(predicate);
        }

        private IQueryable<Package>? BuildSortPackage(IQueryable<Package>? queries, IBaseSort? sort)
        {
            if (queries == null || sort == null) return queries;

            switch (sort.SortType)
            {
                case SortingEnum.HeightoLow:
                    queries = queries.OrderByDescending(x => x.Price * (decimal)(100 - (x.DiscountRate ?? 0)));
                    break;
                case SortingEnum.LowtoHeigh:
                    queries = queries.OrderBy(x => x.Price * (decimal)(100 - (x.DiscountRate ?? 0)));
                    break;
            }

            return queries;
        }

        public PackageViewModel? GetPackageViewModel(Guid id)
        {
            var package = GetPackage(id.ToString());
            var packageViewModel = _mapper.Map<PackageViewModel>(package);

            if (package == null || packageViewModel == null) return null;

            var productVariantPackage = _productVariantPackageService.GetProductVariantPackages().Where(x => x.PackageId == id);
            if (productVariantPackage != null && productVariantPackage.Any())
            {
                packageViewModel.PackageItems = _productVariantService.GetProductVariants()
                    .AsQueryable()
                    .Include(x => x.Product)
                    .Join(productVariantPackage,
                    x => x.Id,
                    y => y.ProductVariantId,
                    (x, y) => new { productVariant = x, variantPackages = y })
                    .Select(x => new ProductVariantViewModel
                    {
                        Id = x.productVariant.Id,
                        ProductId = x.productVariant.Product.Id,
                        Name = x.productVariant.Product.Name,
                        Code = x.productVariant.Code,
                        Number = x.variantPackages.ItemNumber,
                        PriceAfterDiscount = x.productVariant.Price * (decimal)(100 - (x.productVariant.DiscountRate ?? 0)) / 100,
                        ImageUrl = string.IsNullOrEmpty(x.productVariant.Images)
                                ? x.productVariant.Product.ProductImage.ResolveProductImages().FirstOrDefault()
                                : x.productVariant.Images.ResolveProductImages().FirstOrDefault()
                    }).ToList();
            }

            return packageViewModel;
        }

        public bool DeletePackageItem(Guid id, Guid packageId)
        {
            if (id == Guid.Empty || packageId == Guid.Empty) return false;

            return _productVariantPackageService.DeleteProductVariantPackage(packageId, id);
        }

        public bool UpdatePackageItem(Guid id, Guid packageId, int itemNumber)
        {
            if (id == Guid.Empty || packageId == Guid.Empty) return false;

            var packageItem = _productVariantPackageService.GetProductVariantPackage(packageId, id);
            if (packageItem == null) return false;

            packageItem.ItemNumber = itemNumber;

            return _productVariantPackageService.UpdateProductVariantPackage(packageItem);
        }

        public async Task<bool> CreatePackageItem(Guid id, Guid packageId, int itemNumber)
        {
            if (id == Guid.Empty || packageId == Guid.Empty || itemNumber <= 0) return false;
            var result = await  _productVariantPackageService.CreateProductVariantPackage(new ProductVariantPackage
            {
                ItemNumber = itemNumber,
                PackageId = packageId,
                ProductVariantId = id
            });

            if (result == null) return false;

            return true;
        }

        public ProductVariantPackage? GetPackageItem(Guid id, Guid packageId)
        {
            return _productVariantPackageService.GetProductVariantPackage(packageId, id);
        }

        public bool UpdatePackageItem(ProductVariantPackage productVariantPackage)
        {
            return _productVariantPackageService.UpdateProductVariantPackage(productVariantPackage);
        }
    }
}

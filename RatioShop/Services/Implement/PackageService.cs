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
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMapper _mapper;

        public PackageService(IPackageRepository PackageRepository, IMapper mapper, IProductVariantPackageService productVariantPackageService, IProductVariantService productVariantService, ICategoryService categoryService, IProductCategoryService productCategoryService)
        {
            _packageRepository = PackageRepository;
            _mapper = mapper;
            _productVariantPackageService = productVariantPackageService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
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

            var listPackagesViewModel = _mapper.Map<List<PackageViewModel>>(packages);
            listPackagesViewModel.ForEach(x => x = GetAdditionPackageInfoViewModel(x));

            return new ListPackageViewModel
            {
                Packages = listPackagesViewModel,
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
                                case "Category":
                                    queries = BuildFilterCategory(queries, item);
                                    break;
                            }
                            break;
                        }
                    case FilterType.FreeText:
                        queries = BuildFilterByFreeText(queries, item);
                        break;
                    case FilterType.NumberRange:
                        queries = BuildFilterByNumberRange(queries, item);
                        break;
                }
            }

            return queries.Where(predicate);
        }

        private IQueryable<Package> BuildFilterByFreeText(IQueryable<Package> queries, FacetFilterItem? item)
        {
            if (item == null) return queries;

            var getFieldName = Enum.TryParse(typeof(FieldNameFilter), item.FieldName, true, out var filterEnum);
            if (!getFieldName) return queries;

            switch (filterEnum)
            {
                case FieldNameFilter.Name:
                    {
                        var searchText = item.Value;
                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            var fullSearchTextResult = queries.Where(x => x.Code.ToLower().Contains(searchText)
                                            || x.Name.ToLower().Contains(searchText)
                                            || x.ProductFriendlyName.ToLower().Contains(searchText));

                            if (fullSearchTextResult.Count() == 0)
                            {
                                var predicate = PredicateBuilder.False<Package>();

                                var listSearchText = searchText.Trim().ToLower().Split(" ").Select(x => x.Trim()).ToList();
                                if (listSearchText != null && listSearchText.Any())
                                {
                                    foreach (var text in listSearchText)
                                    {
                                        predicate = predicate.Or(x => x.Code.ToLower().Contains(text)
                                                || x.Name.ToLower().Contains(text)
                                                || x.ProductFriendlyName.ToLower().Contains(text));
                                    }
                                }
                                queries = queries.Where(predicate);
                            }
                            else queries = fullSearchTextResult;
                        }
                        break;
                    }
            }
            return queries;
        }

        private IQueryable<Package>? BuildSortPackage(IQueryable<Package>? queries, IBaseSort? sort)
        {
            if (queries == null || sort == null) return queries;

            switch (sort.SortType)
            {
                case SortingEnum.HeightoLow:
                    {
                        var packages = queries.Join(_productVariantPackageService.GetProductVariantPackages().AsQueryable().Include(x => x.ProductVariant),
                            x => x.Id,
                            y => y.PackageId,
                            (x, y) => new
                            {
                                Packages = x,
                                VariantPackages = y,
                                variantPackagesPrice = y.ItemNumber * (y.ProductVariant.Price * (decimal)(100 - (y.ProductVariant.DiscountRate ?? 0)) / 100),
                                manualPackagePrice = x.Price
                            })
                            .GroupBy(x => x.Packages.Id)
                            .Select(x => new
                            {
                                packageId = x.Key,
                                packagePrice = x.Select(y => y).FirstOrDefault().manualPackagePrice != null ? x.Select(y => y).FirstOrDefault().manualPackagePrice : x.Sum(y => y.variantPackagesPrice)
                            });

                        var packageIdsOrdered = packages.OrderByDescending(x => x.packagePrice).Select(x => x.packageId);

                        queries = packageIdsOrdered.Join(queries,
                            x => x,
                            y => y.Id,
                            (x, y) => y).AsQueryable();

                        break;
                    }
                case SortingEnum.LowtoHeigh:
                    {
                        var packages = queries.Join(_productVariantPackageService.GetProductVariantPackages().AsQueryable().Include(x => x.ProductVariant),
                            x => x.Id,
                            y => y.PackageId,
                            (x, y) => new
                            {
                                Packages = x,
                                VariantPackages = y,
                                variantPackagesPrice = y.ItemNumber * (y.ProductVariant.Price * (decimal)(100 - (y.ProductVariant.DiscountRate ?? 0)) / 100),
                                manualPackagePrice = x.Price
                            })
                            .GroupBy(x => x.Packages.Id)
                            .Select(x => new
                            {
                                packageId = x.Key,
                                packagePrice = x.Select(y => y).FirstOrDefault().manualPackagePrice != null ? x.Select(y => y).FirstOrDefault().manualPackagePrice : x.Sum(y => y.variantPackagesPrice)
                            });

                        var packageIdsOrdered = packages.OrderBy(x => x.packagePrice).Select(x => x.packageId);

                        queries = packageIdsOrdered.Join(queries,
                            x => x,
                            y => y.Id,
                            (x, y) => y).AsQueryable();

                        break;
                    }
            }

            return queries;
        }

        private IQueryable<Package> BuildFilterCategory(IQueryable<Package> queries, FacetFilterItem? item)
        {
            if (item == null || string.IsNullOrEmpty(item.Value)) return queries;

            var getCategory = int.TryParse(item.Value, out var categoryId);
            if (!getCategory || categoryId == 0) return queries;

            var packages = queries.Join(_productVariantPackageService.GetProductVariantPackages().AsQueryable().Include(x => x.ProductVariant),
                        x => x.Id,
                        y => y.PackageId,
                        (x, y) => new
                        {
                            Packages = x,
                            ProductIds = y.ProductVariant.ProductId,
                        });

            var productIds = _categoryService.GetListCategoriesChildren(categoryId, true)
                        .Join(_productCategoryService.GetProductCategorys(),
                        x => x.Id,
                        y => y.CategoryId,
                        (x, y) => new { ProductCategory = y })
                        .Select(x => x.ProductCategory.ProductId)
                        .Distinct();

            queries = packages
                .Where(x => productIds.Any(y => y == x.ProductIds))
                .Select(x => x.Packages)
                .Distinct();

            return queries;
        }

        private IQueryable<Package> BuildFilterByNumberRange(IQueryable<Package> queries, FacetFilterItem? item)
        {
            if (item == null) return queries;

            var getFieldName = Enum.TryParse(typeof(FieldNameFilter), item.FieldName, true, out var filterEnum);
            if (!getFieldName) return queries;

            switch (filterEnum)
            {
                case FieldNameFilter.Price:
                    {
                        var packages = queries.Join(_productVariantPackageService.GetProductVariantPackages().AsQueryable().Include(x => x.ProductVariant),
                            x => x.Id,
                            y => y.PackageId,
                            (x, y) => new
                            {
                                Packages = x,
                                VariantPackages = y,
                                variantPackagesPrice = y.ItemNumber * (y.ProductVariant.Price * (decimal)(100 - (y.ProductVariant.DiscountRate ?? 0)) / 100),
                                manualPackagePrice = x.Price
                            })
                            .GroupBy(x => x.Packages.Id)
                            .Select(x => new
                            {
                                packageId = x.Key,
                                packagePrice = x.Select(y => y).FirstOrDefault().manualPackagePrice != null ? x.Select(y => y).FirstOrDefault().manualPackagePrice : x.Sum(y => y.variantPackagesPrice)
                            });

                        var priceRange = item.Value.Split("-");
                        if (priceRange.Length == 2)
                        {
                            var getPriceFrom = decimal.TryParse(priceRange[0], out var priceFrom);
                            var getPriceTo = decimal.TryParse(priceRange[1], out var priceTo);
                            if (!getPriceFrom || !getPriceTo) break;

                            var packageIds = packages.Where(x => x.packagePrice >= priceFrom && x.packagePrice <= priceTo).Select(x => x.packageId);
                            queries = queries.Where(x => packageIds.Any(y => y == x.Id));
                        }
                        else if (priceRange.Length == 1)
                        {
                            var getPriceFrom = decimal.TryParse(priceRange[0], out var priceFrom);

                            if (!getPriceFrom) break;

                            var packageIds = packages.Where(x => x.packagePrice >= priceFrom).Select(x => x.packageId);
                            queries = queries.Where(x => packageIds.Any(y => y == x.Id));
                        }
                        break;
                    }
            }
            return queries;
        }

        public PackageViewModel? GetPackageViewModel(Guid id)
        {
            var package = GetPackage(id.ToString());
            var packageViewModel = _mapper.Map<PackageViewModel>(package);

            if (package == null || packageViewModel == null) return null;

            var productVariantPackage = _productVariantPackageService.GetProductVariantPackages();
            packageViewModel.PackageItems = _productVariantService.GetProductVariants()
                .AsQueryable()
                .Include(x => x.Product)
                .Join(productVariantPackage,
                x => x.Id,
                y => y.ProductVariantId,
                (x, y) => new { productVariant = x, variantPackages = y })
                .Where(x => x.variantPackages.PackageId == id)
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

            if (packageViewModel.PackageItems != null && packageViewModel.PackageItems.Any())
                packageViewModel.AutoCalculatedPrice = packageViewModel.PackageItems.Sum(x => x.PriceAfterDiscount * x.Number) * (decimal)(100 - (packageViewModel.DiscountRate ?? 0)) / 100;

            return packageViewModel;
        }

        public PackageViewModel? GetAdditionPackageInfoViewModel(PackageViewModel packageViewModel)
        {
            var productVariantPackage = _productVariantPackageService.GetProductVariantPackages();
            packageViewModel.PackageItems = _productVariantService.GetProductVariants()
                .AsQueryable()
                .Include(x => x.Product)
                .Join(productVariantPackage,
                x => x.Id,
                y => y.ProductVariantId,
                (x, y) => new { productVariant = x, variantPackages = y })
                .Where(x => x.variantPackages.PackageId == packageViewModel.Id)
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

            if (packageViewModel.PackageItems != null && packageViewModel.PackageItems.Any())
                packageViewModel.AutoCalculatedPrice = packageViewModel.PackageItems.Sum(x => x.PriceAfterDiscount * x.Number) * (decimal)(100 - (packageViewModel.DiscountRate ?? 0)) / 100;

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
            var result = await _productVariantPackageService.CreateProductVariantPackage(new ProductVariantPackage
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

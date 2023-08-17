using AutoMapper;
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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantService _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductVariantStockService _productVariantStockService;
        private readonly IStockService _stockService;
        private readonly IPackageService _packageService;

        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IProductVariantService productVariantService, ICategoryService categoryService, IProductCategoryService productCategoryService, IProductVariantStockService productVariantStockService, IStockService stockService, IMapper mapper, IPackageService packageService)
        {
            _productRepository = productRepository;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _productVariantStockService = productVariantStockService;
            _stockService = stockService;
            _mapper = mapper;
            _packageService = packageService;
        }

        public async Task<bool> AddProduct(Product product)
        {
            product.CreatedDate = DateTime.UtcNow;
            product.ModifiedDate = DateTime.UtcNow;

            return await _productRepository.AddProduct(product);
        }

        public bool DeleteProduct(Guid productId)
        {
            return _productRepository.DeleteProduct(productId);
        }

        public IEnumerable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            IEnumerable<ProductViewModel> products = _productRepository.GetAllProductsByPageNumber(sortBy, pageNumber, pageSize); ;
            return products;
        }

        public ProductViewModel GetProduct(Guid productId, bool getAdditionInfo = true)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null || product.Product == null)
            {
                var productVariant = _productVariantService.GetProductVariant(productId.ToString());
                if (productVariant != null)
                {
                    productId = productVariant.ProductId;
                    product = _productRepository.GetProduct(productId);
                    product.SelectedVariant = productVariant;                    
                }
            }

            var productVariants = _productVariantService.GetProductVariantsByProductId(productId).ToList();

            product.Product.Variants = productVariants;
            if (product.SelectedVariant == null) product.SelectedVariant = product.Product.Variants?.FirstOrDefault();
            product.SelectedVariantImages = product.SelectedVariant?.Images?.ResolveProductImages();

            if (getAdditionInfo)
            {
                product.AvailableStocks = _stockService.GetStocks().Where(x => x.IsActive).ToDictionary(x => x.Id, y => y.Name);
                //
                var availableCategories = _categoryService.GetCategories().ToList();
                product.AvailableCategories = availableCategories;
                //
                var categories = _productCategoryService.GetCategorysByProductId(productId).ToList();
                product.ProductCategories = categories;
            }

            return product;
        }

        public void GetProductRelatedInformation(ProductViewModel product)
        {
            product.Product.CreatedDate = product.Product.CreatedDate.GetCorrectUTC();
            product.Product.ModifiedDate = product.Product.ModifiedDate.GetCorrectUTC();

            var productId = product.Product.Id;
            var productVariants = _productVariantService.GetProductVariantsByProductId(productId).ToList();

            product.Product.Variants = productVariants;

            product.AvailableStocks = _stockService.GetStocks().Where(x => x.IsActive).ToDictionary(x => x.Id, y => y.Name);
            //
            var availableCategories = _categoryService.GetCategories().ToList();
            product.AvailableCategories = availableCategories;
            //
            var categories = _productCategoryService.GetCategorysByProductId(productId).ToList();
            product.ProductCategories = categories;
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize)
        {
            return _productRepository.GetProducts(pageNumber, pageSize);
        }

        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
        {
            IEnumerable<ProductViewModel> products = null;
            if (string.IsNullOrEmpty(sortBy)) products = GetProducts(pageNumber, pageSize);
            products = _productRepository.GetProducts(sortBy, pageNumber, pageSize);
            //

            //
            return products;
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable()
        {
            return _productRepository.GetProducts().Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(int pageNumber, int pageSize)
        {
            return _productRepository.GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(string sortBy, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(sortBy)) return GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);

            return _productRepository.GetProducts(sortBy, pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public List<ProductViewModel> GetProductsRelatedInformation(List<ProductViewModel> products)
        {
            if (products == null || !products.Any()) return products;

            foreach (var product in products)
            {
                GetProductRelatedInformation(product);
            }
            return products;
        }

        public bool UpdateProduct(Product product)
        {
            product.ModifiedDate = DateTime.UtcNow;

            return _productRepository.UpdateProduct(product);
        }        

        public ListProductViewModel GetListProducts(ProductSearchRequest request)
        {
            if (request == null) return new ListProductViewModel();
            
            var searchRequest = _mapper.Map<BaseSearchRequest>(request);

            if (request.IsGetPackages)
            {                
                var packages = _packageService.GetPackages(searchRequest);
                return _mapper.Map<ListProductViewModel>(packages);
            }

            // get products
            var searchProducts = _productRepository.GetAll().Where(x => !x.IsDelete);
            searchProducts = BuildProductFilter(searchProducts, searchRequest);

            searchProducts = searchProducts.SortedBaseProductsGeneric(request.SortType);
            searchProducts = BuildProductSortByPrices(searchProducts, request.SortType);

            var totalMatch = searchProducts.Count();
            searchProducts = searchProducts.PagingProductsGeneric(searchRequest);

            var result = new ListProductViewModel
            {
                PageIndex = searchRequest.PageIndex,
                PageSize = searchRequest.PageSize,
                TotalCount = totalMatch,
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / searchRequest.PageSize),
                Products = GetProductsRelatedInformation(searchProducts.Select(x => new ProductViewModel() { Product = x, ProductDefaultImage = x.ProductImage.ResolveProductImages().FirstOrDefault() }).ToList()),
                FilterItems = searchRequest.FilterItems,
                SortType = searchRequest.SortType
            };

            return result;
        }

        private IQueryable<Product> BuildProductSortByPrices(IQueryable<Product> queries, SortingEnum? sortType)
        {
            if (sortType == null) return queries;

            switch (sortType)
            {
                case SortingEnum.HeightoLow:
                    {
                        var productIds = _productVariantService.GetProductVariants()
                                        .GroupBy(x => x.ProductId)
                                                .Select(g =>
                                                        new
                                                        {
                                                            ProductId = g.Key,
                                                            MinPrice = g.Min(v => v.Price * (decimal)(100 - (v.DiscountRate ?? 0)) / 100),
                                                        })
                                                .OrderByDescending(x => x.MinPrice)
                                                .Select(x => x.ProductId);

                        queries = productIds.Join(queries,
                            x => x,
                            y => y.Id,
                            (x, y) => y).AsQueryable();

                        break;
                    }
                case SortingEnum.LowtoHeigh:
                    {
                        var productIds = _productVariantService.GetProductVariants()
                                        .GroupBy(x => x.ProductId)
                                                .Select(g =>
                                                        new
                                                        {
                                                            ProductId = g.Key,
                                                            MinPrice = g.Min(v => v.Price * (decimal)(100 - (v.DiscountRate ?? 0)) / 100),
                                                        })
                                                .OrderBy(x => x.MinPrice)
                                                .Select(x => x.ProductId);

                        queries = productIds.Join(queries,
                            x => x,
                            y => y.Id,
                            (x, y) => y).AsQueryable();
                        break;
                    }
                default: break;
            }

            return queries;
        }

        private IQueryable<Product> BuildProductFilter(IQueryable<Product> queries, IFacetFilter facetFilter)
        {
            if (facetFilter == null || facetFilter.FilterItems == null || !facetFilter.FilterItems.Any()) return queries;

            foreach (var filter in facetFilter.FilterItems)
            {
                var getfilterType = Enum.TryParse(typeof(FilterType), filter.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.NumberRange:
                        queries = BuildFilterByNumberRange(queries, filter);
                        break;
                    case FilterType.Text:
                        queries = BuildFilterByText(queries, filter);
                        break;
                    case FilterType.FreeText:
                        queries = BuildFilterByFreeText(queries, filter);
                        break;
                }
            }

            return queries;
        }

        #region Filter logic
        private IQueryable<Product> BuildFilterByFreeText(IQueryable<Product> queries, FacetFilterItem? item)
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
                                var predicate = PredicateBuilder.False<Product>();

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

        private IQueryable<Product> BuildFilterByText(IQueryable<Product> queries, FacetFilterItem? item)
        {
            if (item == null) return queries;

            var getFieldName = Enum.TryParse(typeof(FieldNameFilter), item.FieldName, true, out var filterEnum);
            if (!getFieldName) return queries;

            switch (filterEnum)
            {
                case FieldNameFilter.Category:
                    {
                        if (string.IsNullOrEmpty(item.Value))
                            break;

                        var getCategory = int.TryParse(item.Value, out var categoryId);
                        if (!getCategory || categoryId == 0) break;

                        var productIds = _categoryService.GetListCategoriesChildren(categoryId, true)
                                    .Join(_productCategoryService.GetProductCategorys(),
                                    x => x.Id,
                                    y => y.CategoryId,
                                    (x, y) => new { ProductCategory = y })
                                    .Select(x => x.ProductCategory.ProductId)
                                    .Distinct();

                        queries = queries.Where(x => productIds.Any(y => y == x.Id));
                        break;
                    }
            }
            return queries;
        }

        private IQueryable<Product> BuildFilterByNumberRange(IQueryable<Product> queries, FacetFilterItem? item)
        {
            if (item == null) return queries;

            var getFieldName = Enum.TryParse(typeof(FieldNameFilter), item.FieldName, true, out var filterEnum);
            if (!getFieldName) return queries;

            switch (filterEnum)
            {
                case FieldNameFilter.Price:
                    {
                        var priceRange = item.Value.Split("-");
                        if (priceRange.Length == 2)
                        {
                            var getPriceFrom = decimal.TryParse(priceRange[0], out var priceFrom);
                            var getPriceTo = decimal.TryParse(priceRange[1], out var priceTo);
                            if (!getPriceFrom || !getPriceTo) break;

                            var productIds = _productVariantService.GetProductVariants()
                                    .GroupBy(x => x.ProductId)
                                    .Select(g =>
                                            new
                                            {
                                                ProductId = g.Key,
                                                MaxPrice = g.Max(v => v.Price * (decimal)(100 - (v.DiscountRate ?? 0)) / 100),
                                                MinPrice = g.Min(v => v.Price * (decimal)(100 - (v.DiscountRate ?? 0)) / 100),
                                            })
                                    .Where(x => x.MinPrice >= priceFrom && x.MinPrice <= priceTo || x.MaxPrice >= priceFrom && x.MaxPrice <= priceTo)
                                    .Select(x => x.ProductId);

                            queries = queries.Where(x => productIds.Any(y => y == x.Id));
                        }
                        else if (priceRange.Length == 1)
                        {
                            var getPriceFrom = decimal.TryParse(priceRange[0], out var priceFrom);

                            if (!getPriceFrom) break;

                            var productIds = _productVariantService.GetProductVariants()
                                     .GroupBy(x => x.ProductId)
                                     .Select(g =>
                                             new
                                             {
                                                 ProductId = g.Key,
                                                 MaxPrice = g.Max(v => v.Price * (decimal)(100 - (v.DiscountRate) ?? 0) / 100),
                                                 MinPrice = g.Min(v => v.Price * (decimal)(100 - (v.DiscountRate) ?? 0) / 100),
                                             })
                                     .Where(x => x.MaxPrice >= priceFrom)
                                    .Select(x => x.ProductId);

                            queries = queries.Where(x => productIds.Any(y => y == x.Id));
                        }
                        break;
                    }
            }
            return queries;
        }

        public Dictionary<decimal, decimal?> GetProductPriceFilter(decimal range, int maxDepthFilter)
        {
            var results = new Dictionary<decimal, decimal?>();
            if (range == 0 || maxDepthFilter < 2) return results;

            decimal start = 0;
            for (int i = 1; i < maxDepthFilter; i++)
            {
                var partialEnd = start + range;
                results.Add(start, partialEnd);
                start += range;
            }

            results.Add(start, null);

            return results;
        }
        #endregion

        public IEnumerable<ProductViewModel> GetRelatedProducts(ProductViewModel product, int maxNumber = 8)
        {
            if (product == null) return Enumerable.Empty<ProductViewModel>();

            var relatedCategories = product.ProductCategories?.Select(x => x.Id);
            if (relatedCategories == null || !relatedCategories.Any())
            {
                relatedCategories = _productCategoryService.GetCategorysByProductId(product.Product.Id)?.Select(x => x.Id);
            }

            var results = new List<ProductViewModel>();                        

            if (relatedCategories != null && relatedCategories.Any())
            {
                foreach (var item in relatedCategories)
                {
                    if (results.Count < maxNumber)
                    {
                        var numberRemains = maxNumber - results.Count;
                        var relatedProducts = _productCategoryService.GetProductCategorys()
                            .Where(x => x.CategoryId == item 
                                    && x.ProductId != product.Product.Id 
                                    && !results.Any(y=> y.Product.Id == x.ProductId))
                            .ToList()
                            .Select(x => GetProduct(x.ProductId, false)).Take(numberRemains);

                        results.AddRange(relatedProducts);
                    }

                    if (results.Count == maxNumber) break;
                }
            }

            return results;
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;

        private readonly IMapper _mapper;
        private readonly IProductVariantStockService _productVariantStockService;

        public ProductVariantService(IProductVariantRepository ProductVariantRepository, IProductRepository productRepository, IProductVariantStockService productVariantStockService, IMapper mapper)
        {
            _productVariantRepository = ProductVariantRepository;
            _productRepository = productRepository;
            _productVariantStockService = productVariantStockService;
            _mapper = mapper;
        }

        public Task<ProductVariant> CreateProductVariant(ProductVariant ProductVariant)
        {
            ProductVariant.CreatedDate = DateTime.UtcNow;
            ProductVariant.ModifiedDate = DateTime.UtcNow;
            return _productVariantRepository.CreateProductVariant(ProductVariant);
        }

        public bool DeleteProductVariant(string id, bool isDeepDelete = false)
        {
            return _productVariantRepository.DeleteProductVariant(id, isDeepDelete);
        }

        public IEnumerable<ProductVariant> GetProductVariants()
        {
            return _productVariantRepository.GetProductVariants();
        }

        public IEnumerable<ProductVariant> GetProductVariantsByProductId(Guid productId, bool isIncludeDeletedVariant = false)
        {
            if (productId == null || productId == Guid.Empty) return Enumerable.Empty<ProductVariant>();

            var productVariants = _productVariantRepository.GetProductVariantsByProductId(productId, isIncludeDeletedVariant).ToList();

            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    var productVariantStocks = _productVariantStockService.GetProductVariantStocksByVariantId(item.Id).ToList();
                    item.ProductVariantStocks = productVariantStocks;
                }
            }
            return productVariants;
        }

        public ProductVariant? GetProductVariant(string id, bool includeProduct = true)
        {
            var productVariant = _productVariantRepository.GetProductVariant(id);
            if (productVariant == null) return null;

            if (includeProduct)
                productVariant.Product = _productRepository.GetProduct(productVariant.ProductId).Product;

            return productVariant;
        }

        public bool UpdateProductVariant(ProductVariant productVariant)
        {
            productVariant.ModifiedDate = DateTime.UtcNow;
            return _productVariantRepository.UpdateProductVariant(productVariant);
        }

        /// <summary>
        /// Update product variant number = sum product variant number in stock.
        /// </summary>
        /// <param name="variantId"></param>
        /// <returns></returns>
        public bool UpdateProductVariantNumber(Guid variantId)
        {
            if (variantId == Guid.Empty) return false;

            var inStockNumber = _productVariantStockService.GetProductVariantStocksByVariantId(variantId).Sum(x => x.ProductNumber);
            var productVariant = GetProductVariant(variantId.ToString(), false);
            if (productVariant == null) return false;

            productVariant.Number = inStockNumber;
            return UpdateProductVariant(productVariant);
        }
        public bool UpdateProductVariantNumber(ProductVariant productVariant)
        {
            if (productVariant == null) return false;

            var inStockNumber = _productVariantStockService.GetProductVariantStocksByVariantId(productVariant.Id)?.Sum(x => x.ProductNumber);

            productVariant.Number = inStockNumber;
            return UpdateProductVariant(productVariant);
        }

        public bool ReduceProductVariantNumber(Guid variantId, int numberReduce, List<CartStockItem>? cartStockItems)
        {
            var variant = _productVariantRepository.GetProductVariant(variantId.ToString());
            if (variant == null || variant.Number < numberReduce) return false;

            // reduce on productVariantStock table
            var productVariantStockSuccess = _productVariantStockService.ReduceProductVariantStockByNumber(variantId, cartStockItems);
            if (!productVariantStockSuccess) return false;

            // reduce on product variant table            
            return UpdateProductVariantNumber(variant);
        }

        public bool RevertProductVariantNumber(Guid variantId, List<CartStockItem>? cartStockItems)
        {
            var revertOnStock = _productVariantStockService.RevertProductVariantStockByNumber(variantId, cartStockItems);
            if (!revertOnStock) return false;

            return UpdateProductVariantNumber(variantId);
        }

        public List<ProductVariantStock> GetVariantStocksByStockId(int stockId)
        {
            return _productVariantStockService.GetProductVariantStocks().AsQueryable().Where(x => x.StockId == stockId).ToList();
        }

        public List<ProductVariantStock> GetVariantStocksOrderByStocks(List<Stock> stocks)
        {
            var result = new List<ProductVariantStock>();
            if (stocks == null || stocks.Count == 0) return result;

            foreach (var item in stocks)
            {
                result.AddRange(GetVariantStocksByStockId(item.Id));
            }
            return result;
        }

        public ListProductVariantViewModel GetListVariants(BaseSearchRequest args)
        {
            if (args == null) return new ListProductVariantViewModel();

            var productVariants = _productVariantRepository.GetProductVariants()
                .AsQueryable()
                .Include(x => x.Product)
                .Where(x=> x.Product != null && !x.Product.IsDelete && !x.IsDelete)                
                .Select(x => x);

            productVariants = BuildPackageFilters(productVariants, args);

            productVariants = productVariants?.SortedBaseProductsGeneric(args.SortType);
            productVariants = BuildSortVariant(productVariants, args);           

            var totalCount = productVariants?.Count() ?? 0;
            productVariants = productVariants?.PagingProductsGeneric(args);

            return new ListProductVariantViewModel
            {
                ProductVariants = _mapper.Map<List<ProductVariantViewModel>>(productVariants),
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
        }

        private IQueryable<ProductVariant>? BuildPackageFilters(IQueryable<ProductVariant>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.FreeText:
                        queries = BuildFilterByFreeText(queries, item);
                        break;
                }
            }

            return queries;
        }

        private IQueryable<ProductVariant> BuildFilterByFreeText(IQueryable<ProductVariant> queries, FacetFilterItem? item)
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
                                            || x.Product.Name.ToLower().Contains(searchText)
                                            || x.Product.ProductFriendlyName.ToLower().Contains(searchText));

                            if (fullSearchTextResult.Count() == 0)
                            {
                                var predicate = PredicateBuilder.False<ProductVariant>();

                                var listSearchText = searchText.Trim().ToLower().Split(" ").Select(x => x.Trim()).ToList();
                                if (listSearchText != null && listSearchText.Any())
                                {
                                    foreach (var text in listSearchText)
                                    {
                                        predicate = predicate.Or(x => x.Code.ToLower().Contains(text)
                                                || x.Product.Name.ToLower().Contains(text)
                                                || x.Product.ProductFriendlyName.ToLower().Contains(text));
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

        private IQueryable<ProductVariant>? BuildSortVariant(IQueryable<ProductVariant>? queries, IBaseSort? sort)
        {
            if (queries == null || sort == null) return queries;

            switch (sort.SortType)
            {
                case SortingEnum.HeightoLow:
                    return queries.OrderByDescending(x => x.Price * (decimal)(100 - (x.DiscountRate ?? 0)));                    
                case SortingEnum.LowtoHeigh:
                    return queries.OrderBy(x => x.Price * (decimal)(100 - (x.DiscountRate ?? 0)));                    
            }
            
            return queries;
        }
    }
}

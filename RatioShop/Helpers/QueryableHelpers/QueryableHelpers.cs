using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using System.Linq.Expressions;

namespace RatioShop.Helpers.QueryableHelpers
{
    public static class QueryableHelpers
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }        

        public static IQueryable<T> SortedProductsGeneric<T>(this IQueryable<T> queries, SortingEnum? sortBy) where T : BaseProduct
        {
            switch (sortBy)
            {
                case SortingEnum.Default:
                    return queries.OrderByDescending(nameof(BaseProduct.CreatedDate));
                case SortingEnum.Oldest:
                    return queries.OrderBy(nameof(BaseProduct.CreatedDate));                
                case SortingEnum.RecentUpdate:                    
                    return queries.OrderByDescending(nameof(BaseProduct.ModifiedDate));
                default: return queries.OrderByDescending(nameof(BaseProduct.CreatedDate));
            }
        }

        public static IQueryable<T> PagingProductsGeneric<T>(this IQueryable<T> queries, IBasePagingRequest pagingRequest)
        {
            pagingRequest.PageIndex = pagingRequest.PageIndex == 0 ? 1 : pagingRequest.PageIndex;
            pagingRequest.PageSize = pagingRequest.PageSize == 0 ? 5 : pagingRequest.PageSize;

            if (pagingRequest.IsSelectPreviousItems)
            {
                queries = queries.Take(pagingRequest.PageIndex * pagingRequest.PageSize);
            }
            else
            {
                queries = queries.Skip((pagingRequest.PageIndex - 1) * pagingRequest.PageSize)
                .Take(pagingRequest.PageSize);
            }
            return queries;
        }

        public static IQueryable<Product> BuildProductFilter(this IQueryable<Product> queries, IFacetFilter facetFilter)
        {
            if (facetFilter == null || facetFilter.FilterItems == null || !facetFilter.FilterItems.Any()) return queries;

            foreach (var filter in facetFilter.FilterItems)
            {
                switch (filter.Type)
                {
                    case CommonConstant.FilterType.NumberRange:
                        break;
                    case CommonConstant.FilterType.Text:
                        break;
                    case CommonConstant.FilterType.FreeText:
                        {
                            var searchText = filter.Value;
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
            }

            return queries;
        }

        public static IQueryable<T> SortedProductsAdditionInfo<T>(this IQueryable<T> queries, SortingEnum sortBy) where T : ProductVariant
        {
            switch (sortBy)
            {
                case SortingEnum.HeightToLow:
                    return queries.OrderByDescending(nameof(ProductVariant.Price));
                case SortingEnum.LowToHeight:
                    return queries.OrderBy(nameof(ProductVariant.Price));
                default: return queries;
            }
        }

        public static IQueryable<Product> SortedProducts(this IQueryable<Product> products, string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "default":
                    return products.OrderByDescending(nameof(Product.CreatedDate));
                case "oldest":
                    return products.OrderBy(nameof(Product.CreatedDate));
                case "name":
                    return products.OrderBy(nameof(Product.Name));
                case "recentupdate":
                    return products.OrderByDescending(nameof(Product.ModifiedDate));
                default: return products.OrderByDescending(nameof(Product.CreatedDate));
            }
        }

        public static string GetFreeTextFilter(IEnumerable<FacetFilterItem>? filterItems)
        {
            if(filterItems == null || !filterItems.Any()) return string.Empty;

            return filterItems.FirstOrDefault(x => x.Type == CommonConstant.FilterType.FreeText)?.Value ?? string.Empty;
        }
    }
}

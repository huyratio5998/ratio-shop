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

        public static IQueryable<T> SortedBaseProductsGeneric<T>(this IQueryable<T> queries, SortingEnum? sortBy) where T : BaseProduct
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

        public static IQueryable<T> SortedEntitiesGeneric<T>(this IQueryable<T> queries, SortingEnum? sortBy) where T : BaseEntity
        {
            switch (sortBy)
            {
                case SortingEnum.Default:
                    return queries.OrderByDescending(nameof(BaseEntity.CreatedDate));
                case SortingEnum.Oldest:
                    return queries.OrderBy(nameof(BaseEntity.CreatedDate));
                case SortingEnum.RecentUpdate:
                    return queries.OrderByDescending(nameof(BaseEntity.ModifiedDate));
                default: return queries.OrderByDescending(nameof(BaseEntity.CreatedDate));
            }
        }

        public static IQueryable<T> PagingProductsGeneric<T>(this IQueryable<T> queries, IBasePagingRequest pagingRequest)
        {
            pagingRequest.PageIndex = pagingRequest.PageIndex <= 0 ? 1 : pagingRequest.PageIndex;
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

        public static IQueryable<ProductVariant> SortedProductsAdditionInfo(this IQueryable<ProductVariant> queries, SortingEnum sortBy)
        {
            switch (sortBy)
            {
                case SortingEnum.HeightoLow:
                    return queries.OrderByDescending(nameof(ProductVariant.Price));
                case SortingEnum.LowtoHeigh:
                    return queries.OrderBy(nameof(ProductVariant.Price));
                default: return queries;
            }
        }        

        public static string GetFreeTextFilter(IEnumerable<FacetFilterItem>? filterItems)
        {
            if(filterItems == null || !filterItems.Any()) return string.Empty;

            return filterItems.FirstOrDefault(x => x.Type == FilterType.FreeText.ToString())?.Value ?? string.Empty;
        }
    }
}

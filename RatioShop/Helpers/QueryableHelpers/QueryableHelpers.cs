using RatioShop.Data.Models;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbon.Common
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Adds pagination to query
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">A query for use</param>
        /// <param name="index">Page Index</param>
        /// <param name="size">Page Size for retrieve</param>
        /// <returns>Input query with the related data included.</returns>
        public static IQueryable<TEntity> SkipTake<TEntity>(this IQueryable<TEntity> query, int index, int size) => query.Skip((index - 1) * size).Take(size);

        /// <summary>
        /// Calculates total page count
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">A query for use</param>
        /// <param name="size">Page Size for calculation</param>
        /// <returns>Input query with the related data included.</returns>
        public static int CalculatePageCount<TEntity>(this IQueryable<TEntity> query, int? size) => (int)Math.Ceiling(query.Count() / (double)(size ?? query.Count()));

        /// <summary>
        /// Orders data by given values and directions
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">A query for use</param>
        /// <param name="ordination">Used for ordering <see cref="Orderable"/></param>
        /// <returns>Input query with the related data included.</returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query,
            ICollection<Orderable> ordination)
        {
            if (ordination is { Count: 0 })
                return query;

            var expression = query.Expression;
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            int count = 0;
            for (int i = 0; i < ordination.Count; i++)
            {
                var item = ordination.ElementAt(i);
                Expression propertyAccess = parameter;

                foreach (var prop in item.Value.Split('.'))
                {
                    var propInfo = propertyAccess.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                    {
                        propertyAccess = null;
                        break;
                    }

                    propertyAccess = Expression.Property(propertyAccess, propInfo);
                }

                if (propertyAccess == null) continue;

                var lambda = Expression.Lambda(propertyAccess, parameter);

                string methodName;

                if (i == 0)
                    methodName = item.IsAscending ? "OrderBy" : "OrderByDescending";
                else
                    methodName = item.IsAscending ? "ThenBy" : "ThenByDescending";

                expression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(TEntity), propertyAccess.Type },
                    expression,
                    Expression.Quote(lambda)
                );
                count++;
            }
            return count > 0 ? query.Provider.CreateQuery<TEntity>(expression) : query;
        }
    }
}
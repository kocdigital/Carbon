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
            if (ordination == null)
            {
                return query;
            }

            int count = 0;
            var expression = query.Expression;

            foreach (var item in ordination)
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                Expression selector = parameter;

                foreach (var propName in item.Value.Split('.'))
                {
                    var propInfo = selector.Type.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null) continue;

                    selector = Expression.Property(selector, propInfo);
                }

                if (selector == parameter) continue;

                var lambda = Expression.Lambda(selector, parameter);

                var methodName = item.IsAscending ? "OrderBy" : "OrderByDescending";
                var resultExpression = Expression.Call(
                    typeof(Queryable), methodName,
                    new Type[] { query.ElementType, selector.Type },
                    expression, Expression.Quote(lambda));

                expression = resultExpression;
                count++;
            }

            return count > 0 ? query.Provider.CreateQuery<TEntity>(expression) : query;
        }
    }
}
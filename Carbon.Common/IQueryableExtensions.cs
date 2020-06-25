using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Carbon.Common
{
   
    public static class IQueryableExtensions
    {
        //
        // Summary:
        //   The method retrieves paginated data
        //
        // Parameters:
        //   index:
        //     Pagination index
        //
        //   size:
        //     Page size
        //
        // Type parameters:
        //   TEntity:
        //     The type of entity being queried.
        //
        // Returns:
        //     A new query with the related data included.
        public static IQueryable<TEntity> SkipTake<TEntity>(this IQueryable<TEntity> query, int index, int size)
        {
            return query.Skip((index - 1) * size).Take(size);
        }

        //
        // Summary:
        //   The method calculates total page count
        //
        public static int CalculatePageCount<TEntity>(this IQueryable<TEntity> query, int? size)
        {
            return (int)Math.Ceiling(query.Count() / (double)size);
        }

        //
        // Summary:
        //   The method orders data by given values and directions
        //
        // Parameters:
        //   ordination:
        //     order by rules
        //
        // Type parameters:
        //   TEntity:
        //     The type of entity being queried.
        //
        // Returns:
        //     A new query with the related data included.
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, ICollection<Orderable> ordination)
        {
            if (ordination != null)
            {
                int count = 0;
                var expression = query.Expression;

                foreach (var item in ordination)
                {
                    var property = query.ElementType.GetProperty(item.Value);
                    if (property == null) continue;

                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var selector = Expression.PropertyOrField(parameter, item.Value);

                    if (item.IsAscending)
                    {
                        expression = Expression.Call(typeof(Queryable), "OrderBy",
                           new Type[] { query.ElementType, selector.Type },
                           expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                        count++;

                    }
                    else
                    {
                        expression = Expression.Call(typeof(Queryable), "OrderByDescending",
                           new Type[] { query.ElementType, selector.Type },
                           expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                        count++;
                    }

                }
                return count > 0 ? query.Provider.CreateQuery<TEntity>(expression) : query;
            }

            return query;
    }
}

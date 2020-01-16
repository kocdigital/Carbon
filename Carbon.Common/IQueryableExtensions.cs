﻿using System;
using System.Collections.Generic;
using System.Linq;

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
                foreach (var item in ordination)
                {
                    if (item.IsAscending)
                    {
                        query.OrderBy(x => item.Value);
                    }
                    else
                    {
                        query.OrderByDescending(x => item.Value);
                    }
                }
            }

            return query;
        }

    }
}

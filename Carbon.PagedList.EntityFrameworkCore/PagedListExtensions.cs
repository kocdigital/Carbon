﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.PagedList.EntityFrameworkCore
{
    /// <summary>
    /// Container for extension methods designed to simplify the creation of instances of <see cref="PagedList{T}"/>.
    /// </summary>
    public static class PagedListExtensions
    {
        /// <summary>
        /// Creates a subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets. If the collection implements <see cref="IQueryable{T}"/>, it will be treated as such.</param>
        /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
        /// <param name="pageSize">The maximum size of any individual subset.</param>
        /// <returns>A subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.</returns>
        /// <seealso cref="PagedList{T}"/>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            var isAllDataRequest = pageSize == 0 && pageNumber == 1;
            
            var totalItemCount = superset?.Count() ?? 0;
            var result = new List<T>();

            if (superset != null && totalItemCount > 0)
            {
                if (isAllDataRequest)
                {
                    result = await superset.ToListAsync();
                }
                else
                {
                    var skipCount = pageNumber == 1 ? 0 : (pageNumber - 1) * pageSize;
                    result = await superset.Skip(skipCount).Take(pageSize).ToListAsync();
                }
            }

            return new PagedList<T>(result, pageNumber, pageSize, totalItemCount);
        }
    }
}

using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.PagedList.Mapster
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// creates the object and maps values to it
        /// </summary>
        /// <typeparam name="T">Type of Instance</typeparam>
        /// <typeparam name="K">Type of Instance</typeparam>
        /// <param name="pagedList"></param>
        /// <returns>new StaticPagedList with given pagelist object</returns>
        public static IPagedList<K> Adapt<T, K>(this IPagedList<T> pagedList)
        {
            return new StaticPagedList<K>(pagedList.ToList().Adapt<List<K>>(), pagedList);
        }
    }
}

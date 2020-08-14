using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.PagedList.Mapster
{
    public static class PagedListExtensions
    {
        public static IPagedList<K> Adapt<T, K>(this IPagedList<T> pagedList)
        {
            return new StaticPagedList<K>(pagedList.ToList().Adapt<List<K>>(), pagedList);
        }
    }
}

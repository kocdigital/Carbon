using System.Collections.Generic;
using System.Linq;

namespace Carbon.Common.UnitTests.StaticWrappers.QueryableExtensionsWrapper
{
    public class QueryableExtensionsWrapper<TEntity> : IQueryableExtensionsWrapper<TEntity>
    {
        public int CalculatePageCount(IQueryable<TEntity> query, int? size)
        {
            return IQueryableExtensions.CalculatePageCount(query, size);
        }

        public IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, ICollection<Orderable> ordination)
        {
            return IQueryableExtensions.OrderBy(query, ordination);
        }

        public IQueryable<TEntity> SkipTake(IQueryable<TEntity> query, int index, int size)
        {
            return IQueryableExtensions.SkipTake(query, index, size);
        }
    }
}

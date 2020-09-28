using System.Collections.Generic;
using System.Linq;

namespace Carbon.Common.UnitTests.StaticWrappers.QueryableExtensionsWrapper
{
    public interface IQueryableExtensionsWrapper<TEntity>
    {
        IQueryable<TEntity> SkipTake(IQueryable<TEntity> query, int index, int size);
        int CalculatePageCount(IQueryable<TEntity> query, int? size);
        IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, ICollection<Orderable> ordination);
    }
}

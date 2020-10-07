using System.Linq;
using System.Threading.Tasks;

namespace Carbon.PagedList.EntityFrameworkCore.UnitTests.StaticWrappers.QueryableExtensionsWrapper
{
    public class QueryableExtensionsWrapper<TEntity> : IQueryableExtensionsWrapper<TEntity>
    {
        public async Task<IPagedList<TEntity>> ToPagedListAsync(IQueryable<TEntity> superset, int pageNumber, int pageSize)
        {
            return await PagedListExtensions.ToPagedListAsync(superset, pageNumber, pageSize);
        }
    }
}
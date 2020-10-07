using System.Linq;
using System.Threading.Tasks;

namespace Carbon.PagedList.EntityFrameworkCore.UnitTests.StaticWrappers.QueryableExtensionsWrapper
{
    public interface IQueryableExtensionsWrapper<TEntity>
    {
        Task<IPagedList<TEntity>> ToPagedListAsync(IQueryable<TEntity> superset, int pageNumber, int pageSize);
    }
}

using Carbon.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.PageList.Mapster.UnitTests.StaticWrappers
{
    public interface IQueryableExtensionsWrapper<TEntity,TOutputEntity>
    {
        IPagedList<TOutputEntity> Adapt(IPagedList<TEntity> superset);
    }
}

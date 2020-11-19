using Carbon.PagedList;


namespace Carbon.PageList.Mapster.UnitTests.StaticWrappers
{
    public class QueryableExtensionsWrapper<TEntity, TOutputEntity> : IQueryableExtensionsWrapper<TEntity, TOutputEntity>
    {
        public IPagedList<TOutputEntity> Adapt(IPagedList<TEntity> superset)
        {
            return PagedList.Mapster.PagedListExtensions.Adapt<TEntity, TOutputEntity>(superset);
        }
    }
}

namespace Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodesWrapper
{
    public interface IHttpStatusCodesWrapper<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
    }
}

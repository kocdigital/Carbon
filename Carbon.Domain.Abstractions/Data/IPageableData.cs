namespace Carbon.Domain.Abstractions.Data
{
    /// <summary>
    ///     An interface intended for serving the contained data in pages.
    /// </summary>
    public interface IPageableData
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}

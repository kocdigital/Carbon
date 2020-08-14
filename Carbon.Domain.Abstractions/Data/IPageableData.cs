namespace Carbon.Domain.Abstractions.Data
{
    public interface IPageableData
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}

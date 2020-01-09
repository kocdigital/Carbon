namespace Carbon.Common
{
    public interface IPageableDto
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}

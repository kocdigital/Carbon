namespace Carbon.Common
{
    public interface IPageableDto
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }

    }
}

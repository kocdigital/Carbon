namespace Carbon.Common
{
    /// <summary>
    /// An interface which indicates that paging is possible
    /// </summary>
    public interface IPageableDto
    {
        /// <summary>
        /// Used for indicating how many data are on the page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Used for indicating which page it is
        /// </summary>
        public int PageIndex { get; set; }
    }
}

using System.Collections.Generic;

namespace Carbon.WebApplication
{
    /// <summary>
    ///  A class for setting up the Cors Policy
    /// </summary>
    public class CorsPolicySettings
    {
        /// <summary>
        /// List of Orgin
        /// </summary>
        public IList<string> Origins { get; set; }
        /// <summary>
        /// A property that indicates Allowing any headers or not.
        /// </summary>
        public bool AllowAnyHeaders { get; set; }
        /// <summary>
        /// A property that indicates Allowing any methods or not.
        /// </summary>
        public bool AllowAnyMethods { get; set; }
        /// <summary>
        /// A property that indicates Allowing any origin or not.
        /// </summary>
        public bool AllowAnyOrigin { get; set; }
        /// <summary>
        /// A property that indicates Exposing headers like X-Paging-PageIndex.
        /// If you use PagedListOk etc. you must set true this so that clients can read the header values
        /// </summary>
        public bool ExposePaginationHeaders { get; set; }
    }
}

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
    }
}

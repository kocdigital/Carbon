using System;
using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// Represents the interface for the ordinated paged data transfer object.
    /// </summary>
    public class OrdinatedPageDto : IOrdinatedPageDto
    {
         /// <summary>
        /// List of Ordination data
        /// </summary>
        public IList<Orderable> Orderables { get; set; } = new List<Orderable>();
        /// <summary>
        /// Size of data of each page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Page index of the requested data
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Constructor of OrdinatedPagedDto that sets PageSize and PageIndex to their default values
        /// </summary>
        public OrdinatedPageDto()
        {
            PageSize = 250;
            PageIndex = 1;
        }
    }
}

using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// Represents the interface for the ordinated paged data transfer object.
    /// </summary>
    public class OrdinatedPageDto : IOrdinatedPageDto
    {
        private IList<Ordination> _ordination = new List<Ordination>();
        private IList<Orderable> _orderables = new List<Orderable>();
        /// <summary>
        /// Orderables property. Assigning a non-empty list also updates Ordination.
        /// </summary>
        public IList<Orderable> Orderables
        {
            get => _orderables;
            set
            {
                _orderables = value ?? new List<Orderable>();
                if (_orderables.Count > 0)
                {
                    _ordination = new List<Ordination>();
                    foreach (var o in _orderables)
                    {
                        _ordination.Add(new Ordination { Value = o.Value, IsAscending = o.IsAscending });
                    }
                }
            }
        }
        /// <summary>
        /// Ordination property. Assigning a non-empty list also updates Orderables.
        /// </summary>
        public IList<Ordination> Ordination
        {
            get => _ordination;
            set
            {
                _ordination = value ?? new List<Ordination>();
                if (_ordination.Count > 0)
                {
                    _orderables = new List<Orderable>();
                    foreach (var o in _ordination)
                    {
                        _orderables.Add(new Orderable { Value = o.Value, IsAscending = o.IsAscending });
                    }
                }
            }
        }
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

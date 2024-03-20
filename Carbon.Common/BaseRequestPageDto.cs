using System.Collections.Generic;

namespace Carbon.Common
{
    public class BaseRequestPageDto : IOrderableDto, IPageableDto
    {
        // TODO : Since there are differences in UI and API requests, there are 2 orderable properties. It is a development waiting to be fixed as a technical debt.
        public IList<Orderable> Ordination { set { Orderables = value; } }
        public IList<Orderable> Orderables { get; set; }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }

        public BaseRequestPageDto()
        {
            this.PageIndex = 1;
            this.PageSize = 250;
            Orderables = Ordination = new List<Orderable>()
            {
                new Orderable()
                {
                    Value = "UpdatedDate",
                    IsAscending = false,
                }
            };
        }
    }
}
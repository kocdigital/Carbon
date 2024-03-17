using System.Collections.Generic;

namespace Carbon.Common
{
    public class BaseRequestPageDto : IOrderableDto, IPageableDto
    {
        public IList<Orderable> Ordination { set { Orderables = value; } }
        public IList<Orderable> Orderables { get; set; }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }

        public BaseRequestPageDto()
        {
            this.PageSize = 250;
            this.PageIndex = 1;
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
using System.Collections.Generic;

namespace Carbon.Common
{
    public interface IOrderableDto
    {
        public IList<Orderable> Orderables { get; set; }
    }

    public class Orderable
    {
        public string Value { get; set; }
        public bool IsAscending { get; set; }
    }
}

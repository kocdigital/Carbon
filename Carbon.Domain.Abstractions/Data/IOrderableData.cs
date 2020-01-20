using System.Collections.Generic;

namespace Carbon.Domain.Abstractions.Data
{
    public interface IOrderableData
    {
        public IList<Orderable> Orderables { get; set; }
    }

}

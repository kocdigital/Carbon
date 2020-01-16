using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.Abstractions.Data
{
    public interface IOrderableData
    {
        public IList<Orderable> Orderables { get; set; }
    }

    public class Orderable
    {
        public string Value { get; set; }
        public bool IsAscending { get; set; }
    }
}

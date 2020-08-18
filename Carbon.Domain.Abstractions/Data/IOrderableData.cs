using Carbon.Common;
using System.Collections.Generic;

namespace Carbon.Domain.Abstractions.Data
{
    /// <summary>
    ///     An interface that contains a list of <code>Orderable</code> objects.
    /// </summary>
    /// <seealso cref="Orderable"/>
    public interface IOrderableData
    {
        public IList<Orderable> Orderables { get; set; }
    }

}

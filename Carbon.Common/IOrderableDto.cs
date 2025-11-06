using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// An interface that contains a list of <code>Orderable</code> objects.
    /// </summary>
    /// <seealso cref="Orderable"/>
    public interface IOrderableDto
    {
        public IList<Orderable> Orderables { get; set; }
        public IList<Ordination> Ordination { get; set; }
    }
    /// <summary>
    /// Used for sort operations
    /// </summary>
    public class Orderable
    {
        /// <summary>
        /// Represents sorting Column
        /// </summary>
        /// <remarks>
        /// In general, used as Name of the Column
        /// </remarks>
        public string Value { get; set; }
        /// <summary>
        /// Indicates order direction. <c>True</c> for ascending, <c>False</c> for descending sorting
        /// </summary>
        public bool IsAscending { get; set; }
    }
    /// <summary>
    /// Used for sort operations
    /// </summary>
    public class Ordination
    {
        /// <summary>
        /// Represents sorting Column
        /// </summary>
        /// <remarks>
        /// In general, used as Name of the Column
        /// </remarks>
        public string Value { get; set; }
        /// <summary>
        /// Indicates order direction. <c>True</c> for ascending, <c>False</c> for descending sorting
        /// </summary>
        public bool IsAscending { get; set; }
    }
}

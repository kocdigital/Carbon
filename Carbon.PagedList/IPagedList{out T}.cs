using System.Collections.Generic;

namespace Carbon.PagedList
{
    /// <summary>
    /// Represents a subset of a collection of objects that can be individually accessed by index and containing metadata about the superset collection of objects this subset was created from.
    /// </summary>
    /// <remarks>
    /// Represents a subset of a collection of objects that can be individually accessed by index and containing metadata about the superset collection of objects this subset was created from.
    /// </remarks>
    /// <typeparam name="T">The type of object the collection should contain.</typeparam>
    /// <seealso cref="IEnumerable{T}"/>
    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        ///<summary>
        /// Gets the element at the specified index.
        ///</summary>
        ///<param name="index">The zero-based index of the element to get.</param>
        T this[int index] { get; }

        ///<summary>
        /// Gets the number of elements contained on this page.
        ///</summary>
        int Count { get; }

        ///<summary>
        /// Gets a non-enumerable copy of this paged list.
        ///</summary>
        ///<returns>A non-enumerable copy of this paged list.</returns>
        IPagedList GetMetaData();
    }
}

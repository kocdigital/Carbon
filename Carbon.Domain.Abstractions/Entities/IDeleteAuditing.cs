using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for recording detailed information about a deletion operation.
    /// </summary>
    public interface IDeleteAuditing
    {
        string DeletedUser { get; set; }
        DateTime? DeletedDate { get; set; }
    }

}

using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for recording detailed information about an insertion operation.
    /// </summary>
    public interface IInsertAuditing
    {
        string InsertedUser { get; set; }
        DateTime? InsertedDate { get; set; }
    }

}

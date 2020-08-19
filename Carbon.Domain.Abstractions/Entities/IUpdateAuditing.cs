using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for recording detailed information about an update operation.
    /// </summary>
    public interface IUpdateAuditing
    {
        string UpdatedUser { get; set; }
        DateTime? UpdatedDate { get; set; }
    }

}

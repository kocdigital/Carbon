using System;
using System.Runtime.CompilerServices;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entity objects with a unique id.
    /// </summary>
    public interface IEntity
    {
        public Guid Id { get; set; }

    }
}

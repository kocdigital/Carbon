using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities that are connected to a tenant.
    /// </summary>
    public interface IMustHaveTenant
    {
        /// <summary>
        /// Id of tenant connected to this entity.
        /// </summary>
        Guid TenantId { get; set; }
    }
}

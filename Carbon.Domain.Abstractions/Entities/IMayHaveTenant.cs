using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities that may or may not be related to a tenant.
    /// </summary>
    public interface IMayHaveTenant
    {
        /// <summary>
        /// Id of tenant connected to this entity.
        /// </summary>
        Guid? TenantId { get; set; }
    }

}

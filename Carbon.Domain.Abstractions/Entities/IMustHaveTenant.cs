using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IMustHaveTenant
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        Guid TenantId { get; set; }
    }
}

using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IMayHaveTenant
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        Guid? TenantId { get; set; }
    }

}

using Carbon.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IHaveOwnership<T>
        where T : IOwnerRelation
    {
        Guid OwnerId { get; set; }
        Guid OrganizationId { get; set; }
        OwnerType OwnerType { get; set; }
        ICollection<T> RelationalOwners { get; set; }
        int GetObjectTypeCode();

    }
    
}

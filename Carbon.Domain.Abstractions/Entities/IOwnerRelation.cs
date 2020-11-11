using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IOwnerRelation
    {
        Guid EntityId { get; set; }
        int EntityCode { get; set; }
    }
}

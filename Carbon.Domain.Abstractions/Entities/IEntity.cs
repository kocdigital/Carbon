using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
    
}

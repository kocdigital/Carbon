using System;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    /// A shortcut of <see cref="Entity{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class Entity : Entity<Guid>, IEntity
    {

    }
}

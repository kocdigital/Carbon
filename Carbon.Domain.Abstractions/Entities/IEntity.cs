using System.Text;

namespace Carbon.Domain.Abstractions.Entities
{

    public interface IEntity<TPrimaryKey> : IEntity
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        TPrimaryKey Id { get; set; }

        /// <summary>
        /// Checks if this entity is transient (not persisted to database and it has not an <see cref="Id"/>).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        bool IsTransient();
    }

    public interface IEntity
    {

    }
    
}

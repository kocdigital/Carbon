namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for soft delete operation on entities.
    /// </summary>
    /// <remarks>
    ///     Soft delete operation marks the entity as "deleted" in the database instead of actually deleting its records completely.
    /// </remarks>
    public interface ISoftDelete
    {
        /// <summary>
        /// Used to mark an Entity as 'Deleted'. 
        /// </summary>
        bool IsDeleted { get; set; }
    }
}

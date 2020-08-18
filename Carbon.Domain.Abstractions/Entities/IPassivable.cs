namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities that have passive and active states.
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// True: This entity is active.
        /// False: This entity is not active.
        /// </summary>
        bool IsActive { get; set; }
    }
}

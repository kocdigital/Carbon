namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities having state information.
    /// </summary>
    /// <seealso cref="EntityStatus"/>
    public interface IHaveState
    {
        EntityStatus State { get; set; }
    }
}

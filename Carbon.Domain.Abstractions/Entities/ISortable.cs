namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities that support sorting operations.
    /// </summary>
    public interface ISortable
    {
        long Order { get; set; }
    }

}

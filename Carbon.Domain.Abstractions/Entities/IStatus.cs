namespace Carbon.Domain.Abstractions.Entities
{
    public interface IHaveState
    {
        EntityStatus Status { get; set; }
    }
}

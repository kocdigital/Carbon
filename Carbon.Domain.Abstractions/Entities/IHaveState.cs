namespace Carbon.Domain.Abstractions.Entities
{
    public interface IHaveState
    {
        EntityStatus State { get; set; }
    }
}

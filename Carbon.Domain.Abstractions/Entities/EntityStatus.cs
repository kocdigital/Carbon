namespace Carbon.Domain.Abstractions.Entities
{
    public enum EntityStatus
    {
        Idle = 0,
        Processing = 1,
        Succeed = 2,
        Failed = 3,
        Cancelled = 4,
        Paused = 5
    }
}

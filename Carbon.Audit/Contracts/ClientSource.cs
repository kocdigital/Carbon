namespace Carbon.Audit.Contracts;

public enum ClientSource
{
    Platform = 0,
    HMI = 1,
    BackgroundJob = 2,
    Migration = 3,
    SystemJob = 4
}
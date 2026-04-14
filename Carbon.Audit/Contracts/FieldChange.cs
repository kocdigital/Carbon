namespace Carbon.Audit.Contracts;

public sealed class FieldChange
{
    public string? Field { get; set; }
    public object? Before { get; set; }
    public object? After { get; set; }
}
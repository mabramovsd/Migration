namespace Migration.Contracts;

public record ServiceHealthStatus
{
    public required string ServiceName { get; init; }
    public required bool IsAvailable { get; init; }
    public string? Version { get; init; }
    public string? Error { get; init; }
}

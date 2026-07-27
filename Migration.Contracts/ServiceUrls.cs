namespace Migration.Contracts;

public record ServiceUrls
{
    public string? Agro { get; init; }
    public string? Shipbuilding { get; init; }
    public string? School { get; init; }
    public string? NurseryHome { get; init; }
}

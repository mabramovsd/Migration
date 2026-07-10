namespace Migration.MigrationTools.Configuration;

public class AppSettings
{
    public string SourceProvider { get; set; } = string.Empty;
    public string SourceConnectionString { get; set; } = string.Empty;
    public string TargetProvider { get; set; } = string.Empty;
    public string TargetConnectionString { get; set; } = string.Empty;
}

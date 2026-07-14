namespace Migration.Contracts;

/// <summary>
/// API Version for Migration Contracts
/// Format: Major.Minor.Patch
/// - Major: Breaking changes
/// - Minor: New features (backwards compatible)
/// - Patch: Bug fixes (backwards compatible)
/// </summary>
public static class ApiVersion
{
    /// <summary>
    /// Current API version - 1.0.0
    /// Initial release with basic employee management
    /// </summary>
    public const string CurrentVersion = "1.0.0";
    
    /// <summary>
    /// Supported API versions
    /// </summary>
    public static readonly string[] SupportedVersions = { "1.0" };
}

namespace Migration.Contracts.DTO;

/// <summary>
/// Request DTO for removing an employee.
/// Allows soft delete, hard delete, and context (e.g., reason, auditing).
/// </summary>
public class RemoveEmployeeRequest
{
    /// <summary>
    /// Employee ID to remove.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Whether to soft-delete (mark IsDeleted = true) or hard-delete (physically remove).
    /// Default: false = hard delete for now.
    /// </summary>
    public bool SoftDelete { get; set; } = false;

    /// <summary>
    /// Optional: reason or metadata for audit trail.
    /// </summary>
    public string? Reason { get; set; }
}
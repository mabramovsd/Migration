using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    /// <summary>
    /// Audit trail record for any entity operations
    /// </summary>
    public class AuditRecord
    {
        public Guid Id { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Entity type (Employee, Company и т.д.)
        /// </summary>
        [Required]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public string Operation { get; set; } = string.Empty;

        public string? UserName { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Old values (JSON)
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// New values (JSON)
        /// </summary>
        public string? NewValues { get; set; }
    }

    /// <summary>
    /// Types of audit operations
    /// </summary>
    public enum AuditOperation
    {
        Create,
        Update,
        Delete
    }
}

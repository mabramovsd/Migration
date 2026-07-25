using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    /// <summary>
    /// Abstract class for employee
    /// </summary>
    public record EmployeeSummaryInfo
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Birth Date
        /// </summary>
        [Required(ErrorMessage = "Birth Date is required")]
        public DateTime BirthDate { get; init; }

        /// <summary>
        /// Employee name
        /// </summary>
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(200, ErrorMessage = "Full Name cannot exceed 200 characters")]
        [MinLength(1, ErrorMessage = "Full Name cannot be empty")]
        public string? FullName { get; init; }

        /// <summary>
        /// Cerrent company
        /// </summary>
        [MaxLength(50, ErrorMessage = "Current Company cannot exceed 50 characters")]
        public string? CurrentCompany { get; init; }

        /// <summary>
        /// Dictionary with additional fields, depends of company
        /// </summary>
        public Dictionary<string, object>? AdditionalData { get; init; }
    }
}

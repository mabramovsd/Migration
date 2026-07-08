using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    /// <summary>
    /// Abstract class for employee
    /// </summary>
    public class EmployeeSummaryInfo
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Birth Date
        /// </summary>
        [Required(ErrorMessage = "Birth Date is required")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Employee name
        /// </summary>
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(200, ErrorMessage = "Full Name cannot exceed 200 characters")]
        [MinLength(1, ErrorMessage = "Full Name cannot be empty")]
        public string? FullName { get; set; }

        /// <summary>
        /// Cerrent company
        /// </summary>
        [MaxLength(50, ErrorMessage = "Current Company cannot exceed 50 characters")]
        public string? CurrentCompany { get; set; }

        /// <summary>
        /// Dictionary with additional fields, depends of company
        /// </summary>
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}

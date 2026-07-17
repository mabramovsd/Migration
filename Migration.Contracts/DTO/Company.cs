using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    public class Company
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(50, ErrorMessage = "Company Name cannot exceed 50 characters")]
        public required string Name { get; set; }

        /// <summary>
        /// Coordinates as WKT string for geography type
        /// </summary>
        public string? Coordinates { get; set; }
    }
}

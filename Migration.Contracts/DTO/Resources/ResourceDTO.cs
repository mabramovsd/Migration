using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Resources
{
    /// <summary>
    /// Resource information for a company
    /// </summary>
    public record ResourceDTO
    {
        /// <summary>
        /// Company Alias
        /// </summary>
        [Required(ErrorMessage = "Company Alias is required")]
        [MaxLength(50, ErrorMessage = "Company Alias cannot exceed 50 characters")]
        public required string Company { get; init; }

        /// <summary>
        /// Resource title/name
        /// </summary>
        [Required(ErrorMessage = "Resource Title is required")]
        [MaxLength(50, ErrorMessage = "Resource Title cannot exceed 50 characters")]
        public required string Title { get; init; }

        /// <summary>
        /// Resource count/amount
        /// </summary>
        [Required(ErrorMessage = "Resource Count is required")]
        public decimal Count { get; init; }

        /// <summary>
        /// Unit of measurement (kg, units, etc.)
        /// </summary>
        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public required string Unit { get; init; }
    }
}

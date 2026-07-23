using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Professions
{
    /// <summary>
    /// Brief information about profession
    /// </summary>
    public class ProfessionDTO
    {
        /// <summary>
        /// Company Alias
        /// </summary>
        [Required(ErrorMessage = "Company Alias is required")]
        [MaxLength(50, ErrorMessage = "Company Alias cannot exceed 50 characters")]
        public required string Company { get; set; }

        /// <summary>
        /// Profession title
        /// </summary>
        [Required(ErrorMessage = "Profession Title is required")]
        [MaxLength(50, ErrorMessage = "Profession Title cannot exceed 50 characters")]
        public required string Title { get; set; }

        /// <summary>
        /// Column in employee table for this profession
        /// </summary>
        [Required(ErrorMessage = "Column name is required")]
        [MaxLength(50, ErrorMessage = "Column name cannot exceed 50 characters")]
        public required string Column { get; set; }
    }
}

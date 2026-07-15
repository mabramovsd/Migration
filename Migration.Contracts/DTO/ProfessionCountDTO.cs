using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    public class ProfessionCountDTO
    {
        /// <summary>
        /// Profession identifier
        /// </summary>
        [Required(ErrorMessage = "Profession Id is required")]
        public Guid Id { get; set; }

        /// <summary>
        /// Profession
        /// </summary>
        [Required(ErrorMessage = "Profession Title is required")]
        [MaxLength(50, ErrorMessage = "Profession Title cannot exceed 50 characters")]
        public required string ProfessionTitle { get; set; }

        /// <summary>
        /// Count employees
        /// </summary>
        public int Count { get; set; }
    }
}

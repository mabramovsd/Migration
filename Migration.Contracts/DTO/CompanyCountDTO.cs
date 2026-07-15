using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    public class CompanyCountDTO
    {
        /// <summary>
        /// Company
        /// </summary>
        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(50, ErrorMessage = "Company Name cannot exceed 50 characters")]
        public required string CompanyName { get; set; }

        /// <summary>
        /// Count employees
        /// </summary>
        public int Count { get; set; }
    }
}

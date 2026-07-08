using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    public class CategoryCountDTO
    {
        /// <summary>
        /// Category
        /// </summary>
        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(50, ErrorMessage = "Category Name cannot exceed 50 characters")]
        public string CategoryName { get; set; }

        /// <summary>
        /// Count employees
        /// </summary>
        public int Count { get; set; }
    }
}

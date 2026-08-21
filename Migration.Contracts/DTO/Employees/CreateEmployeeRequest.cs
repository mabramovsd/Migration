using Migration.Contracts.DTO.Professions;
using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    public record CreateEmployeeRequest
    {
        public string Event { get; init; } = "AddEmployee";

        [Required(ErrorMessage = "Core Data is required")]
        public Employee CoreData { get; init; }

        /// <summary>
        /// List of professions
        /// </summary>
        public Dictionary<string, bool>? Professions { get; init; }

        /// <summary>
        /// Primary profession
        /// </summary>
        public PrimaryProfession? PrimaryProfession { get; init; }
    }
}

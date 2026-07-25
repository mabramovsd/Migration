using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    public record CreateEmployeeRequest
    {
        public string Event { get; init; } = "AddEmployee";

        [Required(ErrorMessage = "Core Data is required")]
        public Employee CoreData { get; init; }

        public Dictionary<string, object>? AdditionalData { get; init; }
    }
}

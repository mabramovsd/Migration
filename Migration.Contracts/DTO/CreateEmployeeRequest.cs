using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO
{
    public class CreateEmployeeRequest
    {
        public string Event { get; set; } = "AddEmployee";

        [Required(ErrorMessage = "Core Data is required")]
        public Employee CoreData { get; set; }

        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}

namespace Migration.Contracts.DTO
{
    public class CreateEmployeeRequest
    {
        public string Event { get; set; } = "AddEmployee";
        public Employee CoreData { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}
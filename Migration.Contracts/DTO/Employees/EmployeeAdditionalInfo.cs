using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    public class EmployeeAdditionalInfo
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Dictionary with additional fields, depends of company
        /// </summary>
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}

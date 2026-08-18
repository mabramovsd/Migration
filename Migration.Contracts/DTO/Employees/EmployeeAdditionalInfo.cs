using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    public record EmployeeAdditionalInfo
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Dictionary with additional fields, depends of company
        /// </summary>
        public Dictionary<string, object>? Professions { get; init; }
    }
}

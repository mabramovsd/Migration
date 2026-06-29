namespace Migration.Contracts.DTO
{
    /// <summary>
    /// Abstract class for employee
    /// </summary>
    public class EmployeeSummaryInfo
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Birth Date
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Employee name
        /// </summary>
        public string? FullName { get; set; }
        /// <summary>
        /// Cerrent company
        /// </summary>
        public string? CurrentCompany { get; set; }
        /// <summary>
        /// Dictionary with additional fields, depends of company
        /// </summary>
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}

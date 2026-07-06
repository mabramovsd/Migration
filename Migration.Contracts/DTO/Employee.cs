namespace Migration.Contracts.DTO
{
    /// <summary>
    /// Abstract class for employee
    /// </summary>
    public class Employee
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
        /// Current company
        /// </summary>
        public string? CurrentCompany { get; set; }
        /// <summary>
        /// Flag to mark disabled employees
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}

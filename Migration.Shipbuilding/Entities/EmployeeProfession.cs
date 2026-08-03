namespace Migration.Shipbuilding.Entities
{
    /// <summary>
    /// Links an employee to a specific profession with hire/fire dates
    /// </summary>
    public class EmployeeProfession
    {
        /// <summary>
        /// Link identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Employee identifier
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Profession identifier
        /// </summary>
        public Guid ProfessionId { get; set; }

        /// <summary>
        /// Date when employee started this profession
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Date when employee left this profession (null if still active)
        /// </summary>
        public DateTime? FireDate { get; set; }

        /// <summary>
        /// Navigation property
        /// </summary>
        public Profession Profession { get; set; } = null!;
    }
}

namespace Migration.Agro.DTO
{
    public class EmployeeAgro
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Flag to mark disabled employees
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Has license to drive tracktor:)
        /// </summary>
        public bool HasTracktorLicense { get; set; }
    }
}

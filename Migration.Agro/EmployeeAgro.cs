namespace Migration.Agro
{
    public class EmployeeAgro
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Has license to drive tracktor:)
        /// </summary>
        public bool HasTracktorLicense { get; set; }
    }
}

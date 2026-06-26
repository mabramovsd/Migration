namespace Migration.Contracts
{
    /// <summary>
    /// Interface for company
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Hire employee to the company
        /// </summary>
        public bool AddEmployee(Employee employee);

        /// <summary>
        /// Fire employee from the company
        /// </summary>
        public bool RemoveEmployee(Employee employee);
    }
}

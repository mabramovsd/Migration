using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;

namespace Migration.Contracts;

/// <summary>
/// Interface for managing employees.
/// Should be implemented by company services.
/// </summary>
public interface ICompanyService
{
    #region Employees

    /// <summary>
    /// Hire employee to the company
    /// </summary>
    Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request);

    /// <summary>
    /// Getting employee info by id
    /// </summary>
    Task<EmployeeAdditionalInfo?> GetEmployeeByIdAsync(Guid employeeId);

    /// <summary>
    /// Getting list of company employees
    /// </summary>
    Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync();

    /// <summary>
    /// Filtered list of employees
    /// </summary>
    Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter);

    /// <summary>
    /// Fire employee from the company
    /// </summary>
    Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request);

    /// <summary>
    /// Update employee data
    /// </summary>
    Task<Guid> UpdateEmployeeAsync(CreateEmployeeRequest request);

    #endregion Employees

    #region Professions

    /// <summary>
    /// Getting list of professions with stats (count of employees)
    /// </summary>
    Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync();

    /// <summary>
    /// Getting list of all available professions
    /// </summary>
    Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync();

    /// <summary>
    /// Getting production norms (profession -> resource relationship)
    /// </summary>
    Task<IEnumerable<ProfessionResourceNormDTO>> GetProfessionResourceNormsAsync();

    #endregion Professions

    #region Resources

    /// <summary>
    /// Getting list of all available resources
    /// </summary>
    Task<IEnumerable<ResourceDTO>> GetResourcesAsync();

    /// <summary>
    /// Getting forecast for production over a period
    /// </summary>
    Task<IEnumerable<ResourceForecastDTO>> GetResourceForecastAsync(int days);

    #endregion Resources
}

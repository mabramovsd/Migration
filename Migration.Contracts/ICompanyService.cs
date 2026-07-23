using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;

namespace Migration.Contracts;

/// <summary>
/// Interface for managing employees.
/// Used by Agro and Shipbuilding services.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Hire employee to the company
    /// </summary>
    Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request);
    
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
    /// Getting list of professions with stats (count of employees)
    /// </summary>
    Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync();

    /// <summary>
    /// Getting list of all available professions
    /// </summary>
    Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync();
}

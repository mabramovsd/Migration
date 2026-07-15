using Migration.Contracts.DTO;

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
    /// Fire employee from the company
    /// </summary>
    Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request);

    /// <summary>
    /// Getting list of professions
    /// </summary>
    Task<IEnumerable<ProfessionCountDTO>> GetProfessionListAsync();
}
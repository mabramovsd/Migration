using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Contracts;
using Migration.Contracts.DTO;

namespace MigrationWeb.Services;

public class HRService
{
    private readonly CoreDBContext _coreDBContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HRService> _logger;

    public HRService(
        CoreDBContext coreDBContext,
        IServiceProvider serviceProvider,
        ILogger<HRService> logger)
    {
        _coreDBContext = coreDBContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public ICompanyService? GetServiceForCompany(string? companyName) =>
        companyName?.ToLowerInvariant() switch
        {
            "agro" => _serviceProvider.GetKeyedService<ICompanyService>("Agro"),
            "shipbuilding" => _serviceProvider.GetKeyedService<ICompanyService>("Shipbuilding"),
            _ => null
        };

    public async Task<IEnumerable<EmployeeSummaryInfo>> GetEmployeeList()
    {
        // Core data
        var employeesFromCore = await _coreDBContext.Employees.ToListAsync();
        if (!employeesFromCore.Any())
        {
            return new List<EmployeeSummaryInfo>();
        }

        // Additional data
        var tasks = new List<Task<IEnumerable<EmployeeAdditionalInfo>>>();
        var companyTypes = employeesFromCore.Select(e => e.CurrentCompany).Distinct().Where(c => !string.IsNullOrEmpty(c));
        
        foreach (var ct in companyTypes)
        {
            var service = GetServiceForCompany(ct);
            if (service is null)
            {
                _logger.LogWarning("No service registered for company type: {CompanyType}", ct);
                continue;
            }
            tasks.Add(service.GetEmployeeListAsync());
        }

        await Task.WhenAll(tasks);

        //Some formatting for code simplifying
        var agroDataById = tasks.Count > 0 ? tasks[0].Result.ToDictionary(x => x.Id) : new Dictionary<Guid, EmployeeAdditionalInfo>();
        var shipDataById = tasks.Count > 1 ? tasks[1].Result.ToDictionary(x => x.Id) : new Dictionary<Guid, EmployeeAdditionalInfo>();

        return employeesFromCore.Select(employee =>
        {
            var companyDict = employee.CurrentCompany switch
            {
                "Agro" => agroDataById,
                "Shipbuilding" => shipDataById,
                _ => null
            };

            return new EmployeeSummaryInfo
            {
                Id = employee.Id,
                FullName = employee.FullName,
                CurrentCompany = employee.CurrentCompany,
                BirthDate = employee.BirthDate,
                AdditionalData = companyDict?.TryGetValue(employee.Id, out var data) == true ? data.AdditionalData : null
            };
        }).ToList();
    }

    public async Task<IEnumerable<CompanyCountDTO>> GetEmployeeCompanyStatistics()
    {
        //Employees grouped by company
        var data = await _coreDBContext.Employees
            .GroupBy(e => e.CurrentCompany == null ? "Unknown" : e.CurrentCompany.ToLower())
            .Select(g => new
            {
                Company = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        int totalCount = data.Sum(x => x.Count);

        // Adding company 'All' as first line
        var finalResult = data.Select(x => new CompanyCountDTO
        {
            CompanyName = x.Company,
            Count = x.Count
        }).ToList();

        finalResult.Insert(0, new CompanyCountDTO { CompanyName = "All", Count = totalCount });

        return finalResult;
    }

    public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
    {
        var employeeId = Guid.NewGuid();
        request.CoreData.Id = employeeId;

        var companyName = request.CoreData.CurrentCompany ?? string.Empty;

        var service = GetServiceForCompany(companyName);
        if (service is null)
        {
            _logger.LogError("No service registered for company: {Company}", companyName);
            throw new InvalidOperationException($"Unknown company type: {companyName}");
        }

        try
        {
            // Core part
            await _coreDBContext.Employees.AddAsync(new Employee
            {
                Id = employeeId,
                FullName = request.CoreData.FullName,
                CurrentCompany = companyName,
                BirthDate = request.CoreData.BirthDate,
            });
            await _coreDBContext.SaveChangesAsync();

            // Special part
            await service.AddEmployeeAsync(request);

            return employeeId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add employee {EmployeeId} to company {Company}", employeeId, companyName);
            throw;
        }
    }

    public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
    {
        var employee = await _coreDBContext.Employees.FindAsync(request.Id);
        if (employee == null) return false;

        var companyName = employee.CurrentCompany ?? string.Empty;

        var service = GetServiceForCompany(companyName);
        if (service == null)
        {
            _logger.LogWarning("No service registered for company '{Company}' of employee {EmployeeId}", companyName, request.Id);
        }

        try
        {
            if (request.SoftDelete)
            {
                employee.IsDeleted = true;
                await _coreDBContext.SaveChangesAsync();
            }
            // Hard delete
            else
            {
                _coreDBContext.Employees.Remove(employee);
                await _coreDBContext.SaveChangesAsync();

                if (service != null)
                {
                    await service.RemoveEmployeeAsync(request);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove employee {EmployeeId}", request.Id);
            return false;
        }
    }
}

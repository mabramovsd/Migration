using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Companies;

namespace MigrationWeb.Services;

public class CompanyService
{
    private readonly CoreDBContext _coreDBContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(
        CoreDBContext coreDBContext,
        IServiceProvider serviceProvider,
        ILogger<CompanyService> logger)
    {
        _coreDBContext = coreDBContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Company>> GetCompanyList()
    {
        return await _coreDBContext.Companies.ToListAsync();
    }
}

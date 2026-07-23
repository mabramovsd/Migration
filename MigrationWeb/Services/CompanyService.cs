using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Professions;

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

    public async Task<IEnumerable<ProfessionDTO>> GetAllProfessions()
    {
        var professions = new List<ProfessionDTO>();
        
        var microservices = new[] { "Agro", "Shipbuilding" };
        
        foreach (var microservice in microservices)
        {
            try
            {
                var companyService = _serviceProvider.GetKeyedService<ICompanyService>(microservice);
                if (companyService != null)
                {
                    var microserviceProfessions = await companyService.GetProfessionsAsync();
                    professions.AddRange(microserviceProfessions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get professions from microservice {Microservice}", microservice);
            }
        }
        
        return professions;
    }
}

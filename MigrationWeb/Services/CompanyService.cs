using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;
using Migration.Contracts.Interfaces;

namespace MigrationWeb.Services;

public class CompanyService
{
    private readonly CoreDBContext _coreDBContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CompanyService> _logger;
    private readonly string[] _microservices = { "Agro", "Shipbuilding" };

    public CompanyService(
        CoreDBContext coreDBContext,
        IServiceProvider serviceProvider,
        ILogger<CompanyService> logger)
    {
        _coreDBContext = coreDBContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private ICompanyService? GetServiceForCompany(string? companyName) =>
        companyName?.ToLowerInvariant() switch
        {
            "agro" => _serviceProvider.GetKeyedService<ICompanyService>("Agro"),
            "shipbuilding" => _serviceProvider.GetKeyedService<ICompanyService>("Shipbuilding"),
            _ => null
        };

    public async Task<IEnumerable<Company>> GetCompanyListAsync()
    {
        return await _coreDBContext.Companies.ToListAsync();
    }

    public async Task<IEnumerable<ProfessionDTO>> GetAllProfessionsAsync()
    {
        var professions = new List<ProfessionDTO>();
        
        foreach (var microservice in _microservices)
        {
            try
            {
                var companyService = GetServiceForCompany(microservice);
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

    /// <summary>
    /// Get all resources from both companies
    /// </summary>
    public async Task<IEnumerable<ResourceDTO>> GetAllResourcesAsync()
    {
        var resources = new List<ResourceDTO>();
        
        foreach (var microservice in _microservices)
        {
            try
            {
                var companyService = GetServiceForCompany(microservice);
                if (companyService != null)
                {
                    var microserviceResources = await companyService.GetResourcesAsync();
                    resources.AddRange(microserviceResources);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get resources from microservice {Microservice}", microservice);
            }
        }
        
        return resources;
    }

    /// <summary>
    /// Get resources for a specific company
    /// </summary>
    public async Task<IEnumerable<ResourceDTO>?> GetResourcesForCompanyAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Enumerable.Empty<ResourceDTO>();
        }

        var service = GetServiceForCompany(companyName);
        if (service == null)
        {
            return Enumerable.Empty<ResourceDTO>();
        }

        try
        {
            return await service.GetResourcesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get resources from microservice {Microservice}", companyName);
            return Enumerable.Empty<ResourceDTO>();
        }
    }

    /// <summary>
    /// Get norms for a specific company
    /// </summary>
    public async Task<IEnumerable<ProfessionResourceNormDTO>?> GetNormsForCompanyAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Enumerable.Empty<ProfessionResourceNormDTO>();
        }

        var service = GetServiceForCompany(companyName);
        if (service == null)
        {
            return Enumerable.Empty<ProfessionResourceNormDTO>();
        }

        try
        {
            return await service.GetProfessionResourceNormsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get norms from microservice {Microservice}", companyName);
            return Enumerable.Empty<ProfessionResourceNormDTO>();
        }
    }

    /// <summary>
    /// Get resource forecast for a specific company
    /// </summary>
    public async Task<IEnumerable<ResourceForecastDTO>?> GetResourceForecastAsync(string companyName, int days)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Enumerable.Empty<ResourceForecastDTO>();
        }

        var service = GetServiceForCompany(companyName);
        if (service == null)
        {
            return Enumerable.Empty<ResourceForecastDTO>();
        }

        try
        {
            return await service.GetResourceForecastAsync(days);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get resiurce forecast from microservice {Microservice}", companyName);
            return Enumerable.Empty<ResourceForecastDTO>();
        }
    }
}

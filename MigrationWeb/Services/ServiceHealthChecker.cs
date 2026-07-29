using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Migration.Contracts;
using Migration.Contracts.DTO.Companies;
using MigrationWeb.Services;

namespace MigrationWeb.Services;

public class ServiceHealthChecker
{
    private readonly ILogger<ServiceHealthChecker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceUrls _serviceUrls;
    private readonly CompanyService _companyService;

    public ServiceHealthChecker(
        ILogger<ServiceHealthChecker> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<ServiceUrls> serviceUrls,
        CompanyService companyService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _serviceUrls = serviceUrls.Value;
        _companyService = companyService;
    }

    public async Task<List<ServiceHealthStatus>> CheckAllServicesAsync()
    {
        var results = new List<ServiceHealthStatus>();

        // 1. Check Core DB via CompanyService.GetCompanyList()
        results.Add(await CheckCoreDbAsync());

        // 2. Check microservices
        var services = new Dictionary<string, string?>
        {
            { "Agro", _serviceUrls.Agro },
            { "Shipbuilding", _serviceUrls.Shipbuilding },
            { "School", _serviceUrls.School },
            { "NurseryHome", _serviceUrls.NurseryHome }
        };

        foreach (var (serviceName, serviceUrl) in services)
        {
            results.Add(await CheckServiceAsync(serviceName, serviceUrl));
        }

        return results;
    }

    private async Task<ServiceHealthStatus> CheckCoreDbAsync()
    {
        try
        {
            var companies = await _companyService.GetCompanyList();
            var count = companies.Count();

            _logger.LogInformation("Core Database is available (found {Count} companies)", count);
            return new ServiceHealthStatus
            {
                ServiceName = "Core Database",
                IsAvailable = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Core Database is not available");
            return new ServiceHealthStatus
            {
                ServiceName = "Core Database",
                IsAvailable = false,
                Error = ex.Message
            };
        }
    }

    private async Task<ServiceHealthStatus> CheckServiceAsync(string serviceName, string? serviceUrl)
    {
        if (string.IsNullOrEmpty(serviceUrl))
        {
            _logger.LogWarning("Service {ServiceName} URL is not configured", serviceName);
            return new ServiceHealthStatus
            {
                ServiceName = serviceName,
                IsAvailable = false,
                Error = "URL not configured"
            };
        }

        try
        {
            var client = _httpClientFactory.CreateClient(serviceName);
            var response = await client.GetAsync("/api/version/version");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Service {ServiceName} is available at {Url}", serviceName, serviceUrl);
                return new ServiceHealthStatus
                {
                    ServiceName = serviceName,
                    IsAvailable = true,
                    Version = content
                };
            }
            else
            {
                _logger.LogWarning("Service {ServiceName} returned status code {StatusCode}", serviceName, response.StatusCode);
                return new ServiceHealthStatus
                {
                    ServiceName = serviceName,
                    IsAvailable = false,
                    Error = $"Status: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Service {ServiceName} is not available at {Url}", serviceName, serviceUrl);
            return new ServiceHealthStatus
            {
                ServiceName = serviceName,
                IsAvailable = false,
                Error = ex.Message
            };
        }
    }
}

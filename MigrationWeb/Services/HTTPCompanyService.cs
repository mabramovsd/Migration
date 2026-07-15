using Migration.Contracts;
using Migration.Contracts.DTO;

namespace MigrationWeb.Services;

/// <summary>
/// HTTP-Facade for company services (Agro, Shipbuilding etc) using REST API
/// </summary>
public class HTTPCompanyService : ICompanyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HTTPCompanyService> _logger;

    public HTTPCompanyService(HttpClient httpClient, ILogger<HTTPCompanyService> logger)
    {        
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeAdditionalInfo>>("api/v1/hr/employees");
            return result ?? Enumerable.Empty<EmployeeAdditionalInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get employees from HTTP service");
            return Enumerable.Empty<EmployeeAdditionalInfo>();
        }
    }

    public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/hr/employees", request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Guid>();
                return content;
            }
            
            _logger.LogError("Failed to add employee via HTTP service. Status code: {StatusCode}", response.StatusCode);
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add employee via HTTP service");
            return Guid.Empty;
        }
    }

    public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
    {
        try
        {
            var url = $"api/v1/hr/employees/{request.Id}?softDelete={request.SoftDelete}";
            var response = await _httpClient.DeleteAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<bool>();
                return content;
            }
            
            _logger.LogError("Failed to remove employee via HTTP service. Status code: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove employee via HTTP service");
            return false;
        }
    }

    public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionListAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProfessionCountDTO>>("api/v1/hr/count-professions");
            return result ?? Enumerable.Empty<ProfessionCountDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get professions from HTTP service");
            return Enumerable.Empty<ProfessionCountDTO>();
        }
    }
}

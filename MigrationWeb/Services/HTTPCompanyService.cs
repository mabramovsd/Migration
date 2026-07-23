using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using System.Text;
using System.Text.Json;

namespace MigrationWeb.Services;

/// <summary>
/// HTTP-Facade for company services (Agro, Shipbuilding etc) using REST API
/// </summary>
public class HTTPCompanyService : ICompanyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HTTPCompanyService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HTTPCompanyService(HttpClient httpClient, ILogger<HTTPCompanyService> logger)
    {        
        _httpClient = httpClient;
        _logger = logger;
        
        // Configure JSON options to not escape Unicode characters (for Cyrillic)
        _jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
    {
        try
        {
            var result = await GetFromJsonAsync<IEnumerable<EmployeeAdditionalInfo>>("api/v1/hr/employees");
            return result ?? Enumerable.Empty<EmployeeAdditionalInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get employees from HTTP service");
            return Enumerable.Empty<EmployeeAdditionalInfo>();
        }
    }

    public async Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter)
    {
        try
        {
            // Build query string from filter
            var queryString = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(filter.Company))
            {
                queryString.Add($"Company={Uri.EscapeDataString(filter.Company)}");
            }
            if (!string.IsNullOrEmpty(filter.Profession))
            {
                queryString.Add($"Profession={Uri.EscapeDataString(filter.Profession)}");
            }
            
            var requestUri = queryString.Count > 0 
                ? $"api/v1/hr/filter?{string.Join("&", queryString)}" 
                : "api/v1/hr/employees";
            
            var result = await GetFromJsonAsync<IEnumerable<EmployeeAdditionalInfo>>(requestUri);
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
            var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/v1/hr/employees", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonSerializer.Deserialize<Guid>(responseContent, _jsonOptions);
                return deserializedContent != default ? deserializedContent : Guid.Empty;
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
                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
                return deserializedContent;
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

    public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
    {
        try
        {
            var result = await GetFromJsonAsync<IEnumerable<ProfessionCountDTO>>("api/v1/professions/stats");
            return result ?? Enumerable.Empty<ProfessionCountDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get professions stats from HTTP service");
            return Enumerable.Empty<ProfessionCountDTO>();
        }
    }

    public async Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync()
    {
        try
        {
            var result = await GetFromJsonAsync<IEnumerable<ProfessionDTO>>("api/v1/professions");
            return result ?? Enumerable.Empty<ProfessionDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get professions from HTTP service");
            return Enumerable.Empty<ProfessionDTO>();
        }
    }

    private async Task<T?> GetFromJsonAsync<T>(string requestUri)
    {
        using var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }
}

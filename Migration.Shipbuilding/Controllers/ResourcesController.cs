using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Resources;
using Migration.Contracts.Interfaces;

namespace Migration.Shipbuilding.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<ResourcesController> _logger;

    public ResourcesController(ICompanyService companyService, ILogger<ResourcesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<ResourceDTO>> Get()
    {
        return await _companyService.GetResourcesAsync();
    }

    [HttpGet("Forecast/{days}")]
    public async Task<IEnumerable<ResourceForecastDTO>> GetForecast(int days)
    {
        return await _companyService.GetResourceForecastAsync(days);
    }
}

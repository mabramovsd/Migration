using Microsoft.AspNetCore.Mvc;
using Migration.Contracts;
using MigrationWeb.Services;

namespace MigrationWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ServiceHealthChecker _healthChecker;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ServiceHealthChecker healthChecker, ILogger<HealthController> logger)
    {
        _healthChecker = healthChecker;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<IEnumerable<ServiceHealthStatus>> GetStatus()
    {
        return await _healthChecker.CheckAllServicesAsync();
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Migration.Contracts;

namespace Migration.Agro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var apiVersion = ApiVersion.CurrentVersion;
        
        return Ok(new
        {
            Service = "Migration.Agro",
            AssemblyVersion = assembly.GetName().Version?.ToString() ?? "unknown",
            ApiVersion = apiVersion,
            Timestamp = DateTime.UtcNow
        });
    }
}

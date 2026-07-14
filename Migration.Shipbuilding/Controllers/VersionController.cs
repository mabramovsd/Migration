using Microsoft.AspNetCore.Mvc;

namespace Migration.Shipbuilding.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        return Ok(new
        {
            Service = "Migration.Shipbuilding",
            Version = "1.0.0",
            ContractsVersion = "1.0.0",
            Timestamp = DateTime.UtcNow
        });
    }
}

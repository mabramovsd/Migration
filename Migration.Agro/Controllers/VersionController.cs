using Microsoft.AspNetCore.Mvc;

namespace Migration.Agro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        return Ok(new
        {
            Service = "Migration.Agro",
            Version = "1.0.0",
            ContractsVersion = "1.0.0",
            Timestamp = DateTime.UtcNow
        });
    }
}

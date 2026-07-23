using Microsoft.AspNetCore.Mvc;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;

namespace Migration.Shipbuilding.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfessionsController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<ProfessionsController> _logger;

    public ProfessionsController(ICompanyService companyService, ILogger<ProfessionsController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet("stats")]
    public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStats()
    {
        return await _companyService.GetProfessionsStatsAsync();
    }

    [HttpGet]
    public async Task<IEnumerable<ProfessionDTO>> GetProfessions()
    {
        return await _companyService.GetProfessionsAsync();
    }
}

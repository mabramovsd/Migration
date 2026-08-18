using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.Interfaces;

namespace Migration.Shipbuilding.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
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

    [HttpGet("Norms")]
    public async Task<IEnumerable<ProfessionResourceNormDTO>> GetNorms()
    {
        return await _companyService.GetProfessionResourceNormsAsync();
    }
}

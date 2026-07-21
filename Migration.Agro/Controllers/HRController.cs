using Microsoft.AspNetCore.Mvc;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;

namespace Migration.Agro.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HRController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<HRController> _logger;

    public HRController(ICompanyService companyService, ILogger<HRController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet("employees")]
    public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployees()
    {
        return await _companyService.GetEmployeeListAsync();
    }

    [HttpGet("Filter")]
    public async Task<IEnumerable<EmployeeAdditionalInfo>> Get([FromQuery] EmployeeFilter filter)
    {
        return await _companyService.GetFilteredEmployees(filter);
    }

    [HttpPost("employees")]
    public async Task<ActionResult<Guid>> AddEmployee([FromBody] CreateEmployeeRequest request)
    {
        var id = await _companyService.AddEmployeeAsync(request);
        return Ok(id);
    }

    [HttpDelete("employees/{id}")]
    public async Task<ActionResult<bool>> RemoveEmployee(Guid id, [FromQuery] bool softDelete = true)
    {
        var request = new RemoveEmployeeRequest { Id = id, SoftDelete = softDelete };
        var result = await _companyService.RemoveEmployeeAsync(request);
        return Ok(result);
    }

    [HttpGet("count-professions")]
    public async Task<IEnumerable<ProfessionCountDTO>> GetCountByProfession()
    {
        return await _companyService.GetProfessionListAsync();
    }
}

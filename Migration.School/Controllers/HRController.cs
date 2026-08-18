using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.Interfaces;

namespace Migration.School.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class HRController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<HRController> _logger;

    public HRController(ICompanyService companyService, ILogger<HRController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpPost("employees")]
    public async Task<ActionResult<Guid>> AddEmployee([FromBody] CreateEmployeeRequest request)
    {
        var id = await _companyService.AddEmployeeAsync(request);
        return Ok(id);
    }

    [HttpGet("employees/{id}")]
    public async Task<EmployeeAdditionalInfo?> GetEmployeeByIdAsync(Guid id)
    {
        return await _companyService.GetEmployeeByIdAsync(id);
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

    [HttpDelete("employees/{id}")]
    public async Task<ActionResult<bool>> RemoveEmployee(Guid id, [FromQuery] bool softDelete = true)
    {
        var request = new RemoveEmployeeRequest { Id = id, SoftDelete = softDelete };
        var result = await _companyService.RemoveEmployeeAsync(request);
        return Ok(result);
    }

    [HttpPut("employees/{id}")]
    public async Task<ActionResult<Guid>> UpdateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var id = await _companyService.UpdateEmployeeAsync(request);
        return Ok(id);
    }
}

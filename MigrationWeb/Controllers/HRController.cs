using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using MigrationWeb.Services;

namespace MigrationWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HRController : ControllerBase
    {
        private readonly ILogger<HRController> _logger;
        private readonly HRService _hrService;

        public HRController(ILogger<HRController> logger, HRService hrService)
        {
            _logger = logger;
            _hrService = hrService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            Guid userId = await _hrService.AddEmployeeAsync(request);
            return Ok(new { message = "Employee successfully created", userId });
        }

        [HttpGet("GetById")]
        public async Task<EmployeeSummaryInfo?> GetById([FromQuery] Guid employeeId)
        {
            return await _hrService.GetEmployeeByIdAsync(employeeId);
        }

        [HttpGet("All")]
        public async Task<IEnumerable<EmployeeSummaryInfo>> GetAll()
        {
            return await _hrService.GetEmployeeListAsync();
        }

        [HttpGet("Filter")]
        public async Task<IEnumerable<EmployeeSummaryInfo>> Get([FromQuery] EmployeeFilter filter)
        {
            return await _hrService.GetFilteredEmployeesAsync(filter);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] RemoveEmployeeRequest request)
        {
            var success = await _hrService.RemoveEmployeeAsync(request);

            if (!success)
                return NotFound(new { message = "Error when deleting employee" });

            return Ok(new
            {
                message = request.SoftDelete
                ? "Employee marked as deleted (soft delete)"
                : "Employee was successfullt removed"
            });
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] CreateEmployeeRequest request)
        {
            Guid userId = await _hrService.UpdateEmployeeAsync(request);
            return Ok(new { message = "Employee successfully updated", userId });
        }

        [HttpGet("Stats/CompanyCounts")]
        public async Task<IEnumerable<CompanyCountDTO>> GetCompanyCounts()
        {
            return await _hrService.GetEmployeeCompanyStatisticsAsync();
        }

        [HttpGet("Stats/ProfessionCounts/{companyName}")]
        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionCounts(string companyName)
        {
            var service = _hrService.GetServiceForCompany(companyName);
            if (service == null)
            {
                return Enumerable.Empty<ProfessionCountDTO>();
            }
            
            return await service.GetProfessionsStatsAsync();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO;
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


        [HttpGet("All")]
        public async Task<IEnumerable<EmployeeSummaryInfo>> GetAll()
        {
            var employeeList = await _hrService.GetEmployeeList();
            return employeeList;
        }

        [HttpGet("Stats/CategoryCounts")]
        public async Task<IEnumerable<CategoryCountDTO>> GetCategoryCounts()
        {
            var stats = await _hrService.GetEmployeeCategoryStatistics();
            return stats;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            Guid userId = await _hrService.AddEmployeeAsync(request);
            return Ok(new { message = "File successfully saved", userId });
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
    }
}

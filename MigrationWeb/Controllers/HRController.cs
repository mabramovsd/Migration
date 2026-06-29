using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO;
using MigrationWeb.Services;

namespace MigrationWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HRController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HRService _hrService;

        public HRController(ILogger<WeatherForecastController> logger, HRService hrService)
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

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            Guid userId = Guid.NewGuid();
            await _hrService.AddEmployeeAsync(request);

            return Ok(new { message = "File successfully saved", userId });
        }
    }
}

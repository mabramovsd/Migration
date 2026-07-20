using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO;
using MigrationWeb.Services;

namespace MigrationWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly CompanyService _companyService;

        public CompanyController(ILogger<CompanyController> logger, CompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }


        [HttpGet("All")]
        public async Task<IEnumerable<Company>> GetAll()
        {
            var companies = await _companyService.GetCompanyList();
            return companies;
        }
    }
}

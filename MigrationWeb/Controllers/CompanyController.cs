using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Professions;
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
            return await _companyService.GetCompanyList();
        }

        [HttpGet("Professions")]
        public async Task<IEnumerable<ProfessionDTO>> GetProfessions()
        {
            return await _companyService.GetAllProfessions();
        }
    }
}

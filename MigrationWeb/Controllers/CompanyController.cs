using Microsoft.AspNetCore.Mvc;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;
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
            return await _companyService.GetCompanyListAsync();
        }

        [HttpGet("Professions")]
        public async Task<IEnumerable<ProfessionDTO>> GetProfessions()
        {
            return await _companyService.GetAllProfessionsAsync();
        }

        [HttpGet("Resources")]
        public async Task<IEnumerable<ResourceDTO>> GetResources()
        {
            return await _companyService.GetAllResourcesAsync();
        }

        [HttpGet("Resources/{companyName}")]
        public async Task<IEnumerable<ResourceDTO>> GetResourcesForCompany(string companyName)
        {
            var resources = await _companyService.GetResourcesForCompanyAsync(companyName);
            return resources ?? Enumerable.Empty<ResourceDTO>();
        }

        [HttpGet("Norms/{companyName}")]
        public async Task<IEnumerable<ProfessionResourceNormDTO>> GetNormsForCompany(string companyName)
        {
            var norms = await _companyService.GetNormsForCompanyAsync(companyName);
            return norms ?? Enumerable.Empty<ProfessionResourceNormDTO>();
        }

        [HttpGet("Resources/Forecast/{companyName}")]
        public async Task<IEnumerable<ResourceForecastDTO>> GetResourceForecast(string companyName, [FromQuery] int days = 30)
        {
            var result = await _companyService.GetResourceForecastAsync(companyName, days);
            return result ?? Enumerable.Empty<ResourceForecastDTO>();
        }
    }
}

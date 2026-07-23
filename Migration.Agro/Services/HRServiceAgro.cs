using Migration.Agro.DTO;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;

namespace Migration.Agro.Services
{
    public class HRServiceAgro : ICompanyService
    {
        private readonly AgroDBContext _dbContext;
        private readonly ILogger<HRServiceAgro> _logger;

        public HRServiceAgro(AgroDBContext dbContext, ILogger<HRServiceAgro> logger) 
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesAgro
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "HasTracktorLicense", employee.HasTracktorLicense }
                    }
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Profession))
            {
                return await GetEmployeeListAsync();
            }

            //Filter by profession
            var professions = await _dbContext.Professions
                .Where(c => c.Title == filter.Profession)
                .Select(p => p.Column).ToListAsync();
            if (!professions.Any())
            {
                return new List<EmployeeAdditionalInfo>();
            }

            //Mapping
            return await _dbContext.EmployeesAgro
                .Where(emp =>
                    emp.HasTracktorLicense && professions.Contains("HasTracktorLicense"))
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "HasTracktorLicense", employee.HasTracktorLicense }
                    }
                })
                .ToListAsync();
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                //Parsing fields
                var hasTracktorLicense = false;
                if (request.AdditionalData.TryGetValue("HasTracktorLicense", out var hasTracktorLicenseObj))
                {
                    hasTracktorLicense = hasTracktorLicenseObj.ToString() == "true";
                }

                //Saving to DB
                await _dbContext.EmployeesAgro.AddAsync(new EmployeeAgro
                {
                    Id = request.CoreData.Id,
                    HasTracktorLicense = hasTracktorLicense
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add agro employee: {ErrorMessage}", ex.Message);
            }

            return request.CoreData.Id;
        }

        public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesAgro.FindAsync(request.Id);
            if (entity == null) return false;

            try
            {
                if (request.SoftDelete)
                {
                    entity.IsDeleted = true;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    _dbContext.EmployeesAgro.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Agro] Failed to remove employee {EmployeeId}: {ErrorMessage}", request.Id, ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            var allEmployees = await _dbContext.EmployeesAgro
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            var professions = await _dbContext.Professions.ToListAsync();

            var data = professions.Select(p => new ProfessionCountDTO
            {
                Id = p.Id,
                ProfessionTitle = p.Title,
                Count = allEmployees.Count(e =>
                    (p.Column == "All") ||
                    (p.Column == "HasTracktorLicense" && e.HasTracktorLicense)
                )
            }).ToList();

            return data;
        }

        public async Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync()
        {
            var professions = await _dbContext.Professions
                .Select(p => new ProfessionDTO
                {
                    Title = p.Title,
                    Column = p.Column
                })
                .ToListAsync();

            return professions;
        }
    }
}

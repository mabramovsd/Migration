using Migration.School.DTO;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;

namespace Migration.School.Services
{
    public class HRServiceSchool : ICompanyService
    {
        private readonly SchoolDBContext _dbContext;
        private readonly ILogger<HRServiceSchool> _logger;

        public HRServiceSchool(SchoolDBContext dbContext, ILogger<HRServiceSchool> logger) 
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesSchool
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter)
        {
            return await GetEmployeeListAsync();
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                //Saving to DB
                await _dbContext.EmployeesSchool.AddAsync(new EmployeeSchool
                {
                    Id = request.CoreData.Id,
                    IsDeleted = request.CoreData.IsDeleted,
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add school employee: {ErrorMessage}", ex.Message);
            }

            return request.CoreData.Id;
        }

        public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesSchool.FindAsync(request.Id);
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
                    _dbContext.EmployeesSchool.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[School] Failed to remove employee {EmployeeId}: {ErrorMessage}", request.Id, ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            return new List<ProfessionCountDTO>();
        }

        public async Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync()
        {
            return new List<ProfessionDTO>();
        }

        public async Task<IEnumerable<ResourceDTO>> GetResourcesAsync()
        {
            return new List<ResourceDTO>();
        }

        public Task<IEnumerable<ProfessionResourceNormDTO>> GetProfessionResourceNormsAsync()
        {
            return Task.FromResult<IEnumerable<ProfessionResourceNormDTO>>(Array.Empty<ProfessionResourceNormDTO>());
        }

        public Task<IEnumerable<ProfessionResourceForecastDTO>> GetProfessionResourceForecastAsync(int days)
        {
            return Task.FromResult<IEnumerable<ProfessionResourceForecastDTO>>(Array.Empty<ProfessionResourceForecastDTO>());
        }
    }
}

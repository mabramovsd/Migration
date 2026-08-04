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

        #region Employees

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

        public async Task<EmployeeAdditionalInfo?> GetEmployeeByIdAsync(Guid employeeId)
        {
            var entity = await _dbContext.EmployeesSchool.FindAsync(employeeId);

            if (entity == null || entity.IsDeleted)
            {
                return null;
            }

            return new EmployeeAdditionalInfo
            {
                Id = entity.Id
            };
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesSchool
                .Where(emp => !emp.IsDeleted)
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

        #endregion Employees

        #region Professions

        public Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            return Task.FromResult<IEnumerable<ProfessionCountDTO>>(Array.Empty<ProfessionCountDTO>());
        }

        public Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync()
        {
            return Task.FromResult<IEnumerable<ProfessionDTO>>(Array.Empty<ProfessionDTO>());
        }

        public Task<IEnumerable<ProfessionResourceNormDTO>> GetProfessionResourceNormsAsync()
        {
            return Task.FromResult<IEnumerable<ProfessionResourceNormDTO>>(Array.Empty<ProfessionResourceNormDTO>());
        }

        #endregion Professions

        #region Resources

        public Task<IEnumerable<ResourceDTO>> GetResourcesAsync()
        {
            return Task.FromResult<IEnumerable<ResourceDTO>>(Array.Empty<ResourceDTO>());
        }

        public Task<IEnumerable<ResourceForecastDTO>> GetResourceForecastAsync(int days)
        {
            return Task.FromResult<IEnumerable<ResourceForecastDTO>>(Array.Empty<ResourceForecastDTO>());
        }

        #endregion Resources
    }
}

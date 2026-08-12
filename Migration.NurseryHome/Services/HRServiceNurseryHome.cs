using Migration.NurseryHome.DTO;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;

namespace Migration.NurseryHome.Services
{
    public class HRServiceNurseryHome : ICompanyService
    {
        private const string ServiceName = "NurseryHome";
        private readonly NurseryHomeDBContext _dbContext;
        private readonly ILogger<HRServiceNurseryHome> _logger;

        public HRServiceNurseryHome(NurseryHomeDBContext dbContext, ILogger<HRServiceNurseryHome> logger) 
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
                await _dbContext.EmployeesNurseryHome.AddAsync(new EmployeeNurseryHome
                {
                    Id = request.CoreData.Id,
                    IsDeleted = request.CoreData.IsDeleted,
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogAddEmployeeError(ServiceName, ex);
            }

            return request.CoreData.Id;
        }

        public async Task<EmployeeAdditionalInfo?> GetEmployeeByIdAsync(Guid employeeId)
        {
            var entity = await _dbContext.EmployeesNurseryHome.FindAsync(employeeId);

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
            return await _dbContext.EmployeesNurseryHome
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
            var entity = await _dbContext.EmployeesNurseryHome.FindAsync(request.Id);
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
                    _dbContext.EmployeesNurseryHome.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogRemoveEmployeeError(ServiceName, request.Id, ex);
                return false;
            }
        }

        public async Task<Guid> UpdateEmployeeAsync(CreateEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesNurseryHome.FindAsync(request.CoreData.Id);
            if (entity == null) return Guid.Empty;

            try
            {
                entity.IsDeleted = request.CoreData.IsDeleted;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogUpdateEmployeeError(ServiceName, request.CoreData.Id, ex);
            }

            return request.CoreData.Id;
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

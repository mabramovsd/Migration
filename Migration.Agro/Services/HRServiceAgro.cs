using Migration.Agro.DTO;
using Migration.Contracts.DTO;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;

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
                .Take(10)
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
            var employeeId = Guid.NewGuid();

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
                    Id = employeeId,
                    HasTracktorLicense = hasTracktorLicense
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add agro employee: {ErrorMessage}", ex.Message);
            }

            return employeeId;
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
    }
}

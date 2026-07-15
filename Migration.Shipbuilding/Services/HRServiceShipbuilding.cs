using Migration.Shipbuilding.DTO;
using Migration.Contracts.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Contracts;

namespace Migration.Shipbuilding.Services
{
    public class HRServiceShipbuilding : ICompanyService
    {
        private readonly ShipbuildingDBContext _dbContext;
        private readonly ILogger<HRServiceShipbuilding> _logger;

        public HRServiceShipbuilding(ShipbuildingDBContext dbContext, ILogger<HRServiceShipbuilding> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesShipbuilding
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "CanDesignShip", employee.CanDesignShip },
                        { "CanCarpentry", employee.CanCarpentry },
                        { "CanWeld", employee.CanWeld },
                    }
                })
                .ToListAsync();
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                //Parsing fields
                var canCarpentry = false;
                if (request.AdditionalData.TryGetValue("CanCarpentry", out var canCarpentryObj))
                {
                    canCarpentry = canCarpentryObj.ToString() == "true";
                }
                var canWeld = false;
                if (request.AdditionalData.TryGetValue("CanWeld", out var canWeldObj))
                {
                    canWeld = canWeldObj.ToString() == "true";
                }
                var canDesignShip = false;
                if (request.AdditionalData.TryGetValue("CanDesignShip", out var canDesignObj))
                {
                    canDesignShip = canDesignObj.ToString() == "true";
                }

                //Saving to DB
                await _dbContext.EmployeesShipbuilding.AddAsync(new EmployeeShipbuilding
                {
                    Id = request.CoreData.Id,
                    CanCarpentry = canCarpentry,
                    CanDesignShip = canDesignShip,
                    CanWeld = canWeld,
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add shipbuilding employee: {ErrorMessage}", ex.Message);
            }

            return request.CoreData.Id;
        }

        public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesShipbuilding.FindAsync(request.Id);
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
                    _dbContext.EmployeesShipbuilding.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Shipbuilding] Failed to delete employee {EmployeeId}: {ErrorMessage}", request.Id, ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionListAsync()
        {
            var allEmployees = await _dbContext.EmployeesShipbuilding
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            var professions = await _dbContext.Professions.ToListAsync();

            var data = professions.Select(p => new ProfessionCountDTO
            {
                Id = p.Id,
                ProfessionTitle = p.Title,
                Count = allEmployees.Count(e =>
                    (p.Column == "All") ||
                    (p.Column == "CanCarpentry" && e.CanCarpentry) ||
                    (p.Column == "CanDesignShip" && e.CanDesignShip) ||
                    (p.Column == "CanWeld" && e.CanWeld)
                )
            }).ToList();

            return data;
        }
    }
}

using Migration.Shipbuilding.DTO;
using Migration.Contracts.DTO;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;

namespace Migration.Shipbuilding.Services
{
    public class HRServiceShipbuilding : ICompanyService
    {
        private readonly ShipbuildingDBContext _dbContext;
        public HRServiceShipbuilding(ShipbuildingDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesShipbuilding
                .Take(10)
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
            var employeeId = Guid.NewGuid();

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
                    Id = employeeId,
                    CanCarpentry = canCarpentry,
                    CanDesignShip = canDesignShip,
                    CanWeld = canWeld,
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // TODO: logging
                Console.WriteLine($"Failed to add shipbuilding employee: {ex.Message}");
            }

            return employeeId;
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
                Console.WriteLine($"[Shipbuilding] Failed to delete employee {request.Id}: {ex}");
                return false;
            }
        }
    }
}

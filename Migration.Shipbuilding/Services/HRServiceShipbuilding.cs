using Migration.Shipbuilding.DTO;
using Migration.Contracts.DTO;

namespace Migration.Shipbuilding.Services
{
    public class HRServiceShipbuilding
    {
        private readonly ShipbuildingDBContext _dbContext;
        public HRServiceShipbuilding(ShipbuildingDBContext dbContext) 
        {
            _dbContext = dbContext;
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
                var s = ex.Message;
            }

            return employeeId;
        }
    }
}

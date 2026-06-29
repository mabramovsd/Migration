using Migration.Agro.Services;
using Migration.Contracts;
using Migration.Contracts.DTO;
using Migration.Shipbuilding.Services;

namespace MigrationWeb.Services
{
    public class HRService
    {
        private readonly HRServiceAgro _hrServiceAgro;
        private readonly HRServiceShipbuilding _hrServiceShipbuilding;
        private readonly CoreDBContext _coreDBContext;
        public HRService(CoreDBContext coreDBContext, HRServiceAgro hrServiceAgro, HRServiceShipbuilding hrServiceShipbuilding)
        { 
            _coreDBContext = coreDBContext;
            _hrServiceAgro = hrServiceAgro;
            _hrServiceShipbuilding = hrServiceShipbuilding;
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            var employeeId = Guid.NewGuid();

            try
            {
                // Core part
                await _coreDBContext.Employees.AddAsync(new Employee 
                { 
                    Id = employeeId, 
                    FullName = request.CoreData.FullName,
                    CurrentCompany = request.CoreData.CurrentCompany,
                    BirthDate = request.CoreData.BirthDate,
                });
                await _coreDBContext.SaveChangesAsync();

                // Special part
                if (request.CoreData.CurrentCompany == "Agro")
                {
                    await _hrServiceAgro.AddEmployeeAsync(request);
                }
                else if (request.CoreData.CurrentCompany == "Shipbuilding")
                {
                    await _hrServiceShipbuilding.AddEmployeeAsync(request);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }

            return employeeId;
        }
    }
}

using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<EmployeeSummaryInfo>> GetEmployeeList()
        {
            // Core data
            var employeesFromCore = await _coreDBContext.Employees.Take(10).ToListAsync();
            if (!employeesFromCore.Any())
            {
                return new List<EmployeeSummaryInfo>();
            }

            // Additional data
            var tasks = new List<Task<IEnumerable<EmployeeAdditionalInfo>>>()
            {
                 _hrServiceAgro.GetEmployeeList(),
                 _hrServiceShipbuilding.GetEmployeeList()
            };
            await Task.WhenAll(tasks);

            //Some formatting for code simplifying
            var agroDataById = tasks[0].Result.ToDictionary(x => x.Id);
            var shipDataById = tasks[1].Result.ToDictionary(x => x.Id);

            return employeesFromCore.Select(employee =>
            {
                var companyDict = employee.CurrentCompany switch
                {
                    "Agro" => agroDataById,
                    "Shipbuilding" => shipDataById,
                    _ => null
                };

                return new EmployeeSummaryInfo
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    CurrentCompany = employee.CurrentCompany,
                    BirthDate = employee.BirthDate,
                    AdditionalData = companyDict?.TryGetValue(employee.Id, out var data) == true ? data.AdditionalData : null
                };
            }).ToList();
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            var employeeId = Guid.NewGuid();
            request.CoreData.Id = employeeId;

            try
            {
                // Core part
                await _coreDBContext.Employees.AddAsync(new Employee 
                { 
                    Id = request.CoreData.Id, 
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

using Migration.Agro.Services;
using Migration.Contracts;
using Migration.Contracts.DTO;

namespace MigrationWeb.Services
{
    public class HRService
    {
        private readonly HRServiceAgro _hrServiceAgro;
        private readonly CoreDBContext _coreDBContext;
        public HRService(CoreDBContext coreDBContext, HRServiceAgro hrServiceAgro) 
        { 
            _coreDBContext = coreDBContext;
            _hrServiceAgro = hrServiceAgro;
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            var employeeId = Guid.NewGuid();

            try
            {
                // Часть 1: Сохраняем в Ядро (всегда выполняется)
                await _coreDBContext.Employees.AddAsync(new Employee 
                { 
                    Id = employeeId, 
                    FullName = request.CoreData.FullName,
                    CurrentCompany = request.CoreData.CurrentCompany,
                    BirthDate = request.CoreData.BirthDate,
                });
                await _coreDBContext.SaveChangesAsync(); // Локальная транзакция для ядра

                // Часть 2: Сохраняем в Специализированный сервис
                if (request.CoreData.CurrentCompany == "Agro")
                {
                    await _hrServiceAgro.AddEmployeeAsync(request);
                }
                // else if for other companies...


            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }

            return employeeId;
        }
    }
}

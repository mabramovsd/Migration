using Migration.Contracts;
using Migration.Contracts.DTO;

namespace MigrationWeb.Services
{
    public class HRService
    {
        private readonly CoreDBContext _coreDBContext;
        public HRService(CoreDBContext coreDBContext) 
        { 
            _coreDBContext = coreDBContext;
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            var employeeId = Guid.NewGuid();

            try
            {
                // Часть 1: Сохраняем в Ядро (всегда выполняется)
                await _coreDBContext.Employees.AddAsync(new Employee { Id = employeeId, FullName = request.CoreData.FullName });
                await _coreDBContext.SaveChangesAsync(); // Локальная транзакция для ядра

                // Часть 2: Сохраняем в Специализированный сервис
                //if (request.CompanyCode == "agro")
                {
                    // Здесь кроется проблема распределенной транзакции.
                    // Простой вызов может упасть, и мы получим несогласованность данных.
                    // Для надежности здесь должен использоваться Outbox/Saga.
                    //await _agroApiClient.AddAgroEmployeeAsync(new AgroEmployeeDto
                    //{
                    //    Id = employeeId,
                    //    HasTracktorLicense = request.HasTracktorLicense ?? false
                    //});
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

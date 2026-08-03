using Migration.Shipbuilding.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Resources;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Migration.Shipbuilding.Services
{
    public class HRServiceShipbuilding : ICompanyService
    {
        private const string ServiceName = "Shipbuilding";
        private const decimal WORK_HOURS_PER_DAY = 5;
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
                    AdditionalData = CreateAdditionalData(employee)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Profession))
            {
                return await GetEmployeeListAsync();
            }

            //Filter by profession
            var employeeIds = await _dbContext.EmployeeProfessions
                .Where(x =>
                    (x.FireDate == null || x.FireDate < DateTime.UtcNow)
                    && x.Profession.Title == filter.Profession
                )
                .Select(x => x.EmployeeId)
                .ToListAsync();
            if (!employeeIds.Any())
            {
                return new List<EmployeeAdditionalInfo>();
            }

            //Mapping
            return await _dbContext.EmployeesShipbuilding
                .Where(emp => employeeIds.Contains(emp.Id))
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    AdditionalData = CreateAdditionalData(employee)
                })
                .ToListAsync();
        }

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                // Parsing fields
                var employee = new EmployeeShipbuilding
                {
                    Id = request.CoreData.Id,
                    CanCarpentry = ParseBool(request.AdditionalData, "CanCarpentry"),
                    CanWeld = ParseBool(request.AdditionalData, "CanWeld"),
                    CanDesignShip = ParseBool(request.AdditionalData, "CanDesignShip"),
                    CanPaint = ParseBool(request.AdditionalData, "CanPaint"),
                    CanRig = ParseBool(request.AdditionalData, "CanRig"),
                    CanShipyard = ParseBool(request.AdditionalData, "CanShipyard")
                };

                //Saving to DB
                await _dbContext.EmployeesShipbuilding.AddAsync(employee);
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

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            var professions = await _dbContext.Professions.ToListAsync();

            var table = _dbContext.EmployeeProfessions
                .Where(x => x.FireDate == null || x.FireDate < DateTime.UtcNow)
                .GroupBy(x => x.Profession.Title)
                .ToDictionary(x => x.Key, x => x.Count());
            table.Add("Все", _dbContext.EmployeesShipbuilding.Count());

            var data = professions.Select(p => new ProfessionCountDTO
            {
                Id = p.Id,
                ProfessionTitle = p.Title,
                Count = table.ContainsKey(p.Title) ? table[p.Title] : 0
            }).ToList();

            return data;
        }

        public async Task<IEnumerable<ProfessionDTO>> GetProfessionsAsync()
        {
            var professions = await _dbContext.Professions
                .Select(p => new ProfessionDTO
                {
                    Company = ServiceName,
                    Title = p.Title,
                    Column = p.Column
                })
                .ToListAsync();

            return professions;
        }

        public async Task<IEnumerable<ResourceDTO>> GetResourcesAsync()
        {
            var resources = await _dbContext.ResourcesShipbuilding
                .Select(r => new ResourceDTO
                {
                    Company = ServiceName,
                    Title = r.Title,
                    Count = r.Count,
                    Unit = r.Unit
                })
                .ToListAsync();

            return resources;
        }

        public async Task<IEnumerable<ProfessionResourceNormDTO>> GetProfessionResourceNormsAsync()
        {
            var norms = await _dbContext.ProfessionResourceNorms
                .Include(n => n.Profession)
                .Include(n => n.Resource)
                .Select(n => new
                {
                    n.Hours,
                    n.QuantityProduced,
                    Profession = n.Profession!.Title,
                    Resource = n.Resource!.Title
                })
                .ToListAsync();

            return norms.Select(n => new ProfessionResourceNormDTO
            {
                Company = ServiceName,
                Profession = n.Profession,
                Resource = n.Resource,
                Hours = n.Hours,
                QuantityProduced = n.QuantityProduced
            });
        }

        public Task<IEnumerable<ResourceForecastDTO>> GetResourceForecastAsync(int days)
        {
            return Task.FromResult<IEnumerable<ResourceForecastDTO>>(Array.Empty<ResourceForecastDTO>());
        }



        #region Helpers

        private static Dictionary<string, object> CreateAdditionalData(EmployeeShipbuilding employee)
        {
            return new Dictionary<string, object>
            {
                { "CanDesignShip", employee.CanDesignShip },
                { "CanCarpentry", employee.CanCarpentry },
                { "CanWeld", employee.CanWeld },
                { "CanShipyard", employee.CanShipyard },
                { "CanPaint", employee.CanPaint },
                { "CanRig", employee.CanRig }
            };
        }

        private static bool ParseBool(Dictionary<string, object> data, string key)
        {
            if (!data.TryGetValue(key, out var value)) return false;
            return value.ToString() == "true";
        }

        private static bool CountByColumn(EmployeeShipbuilding e, string column)
        {
            return column switch
            {
                "CanDesignShip" => e.CanDesignShip,
                "CanCarpentry" => e.CanCarpentry,
                "CanWeld" => e.CanWeld,
                "CanPaint" => e.CanPaint,
                "CanRig" => e.CanRig,
                "CanShipyard" => e.CanShipyard,
                _ => false
            };
        }

        #endregion
    }
}

using Migration.Shipbuilding.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Resources;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq.Expressions;
using Migration.Contracts.Extensions;
using Migration.Contracts.Interfaces;

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

        #region Employees

        public async Task<Guid> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                // Parsing fields
                var employee = new EmployeeShipbuilding
                {
                    Id = request.CoreData.Id,
                    CanCarpentry = ParseBool(request.Professions, "CanCarpentry"),
                    CanWeld = ParseBool(request.Professions, "CanWeld"),
                    CanDesignShip = ParseBool(request.Professions, "CanDesignShip"),
                    CanPaint = ParseBool(request.Professions, "CanPaint"),
                    CanRig = ParseBool(request.Professions, "CanRig"),
                    CanShipyard = ParseBool(request.Professions, "CanShipyard")
                };
                await _dbContext.EmployeesShipbuilding.AddAsync(employee);

                if (request.PrimaryProfession != null)
                {
                    var profession = await _dbContext.Professions.FirstOrDefaultAsync(p => p.Column == request.PrimaryProfession.Column);
                    if (profession != null)
                    {
                        var empProf = new EmployeeProfession
                        {
                            EmployeeId = employee.Id,
                            ProfessionId = profession.Id,
                            HireDate = request.PrimaryProfession.HireDate,
                            FireDate = null
                        };
                        await _dbContext.EmployeeProfessions.AddAsync(empProf);
                    }
                }

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
            var entity = await _dbContext.EmployeesShipbuilding.FindAsync(employeeId);

            if (entity == null || entity.IsDeleted)
            {
                return null;
            }

            return new EmployeeAdditionalInfo
            {
                Id = entity.Id,
                Professions = CreateAdditionalData(entity)
            };
        }

        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetEmployeeListAsync()
        {
            return await _dbContext.EmployeesShipbuilding
                .Where(emp => !emp.IsDeleted)
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    Professions = CreateAdditionalData(employee)
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
                    && x.Profession!.Title == filter.Profession
                )
                .Select(x => x.EmployeeId)
                .ToListAsync();
            if (!employeeIds.Any())
            {
                return new List<EmployeeAdditionalInfo>();
            }

            //Mapping
            return await _dbContext.EmployeesShipbuilding
                .Where(emp => employeeIds.Contains(emp.Id) && !emp.IsDeleted)
                .Select(employee => new EmployeeAdditionalInfo
                {
                    Id = employee.Id,
                    Professions = CreateAdditionalData(employee)
                })
                .ToListAsync();
        }

        public async Task<bool> RemoveEmployeeAsync(RemoveEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesShipbuilding.FindAsync(request.Id);
            if (entity == null) return false;

            try
            {
                var currentProfession = entity.EmployeeProfessions.FirstOrDefault(ep => ep.FireDate == null);
                if (currentProfession != null)
                {
                    currentProfession.FireDate = request.FireDate;
                }

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
                _logger.LogRemoveEmployeeError(ServiceName, request.Id, ex);
                return false;
            }
        }

        public async Task<Guid> UpdateEmployeeAsync(CreateEmployeeRequest request)
        {
            var entity = await _dbContext.EmployeesShipbuilding.FindAsync(request.CoreData.Id);
            if (entity == null) return Guid.Empty;

            try
            {
                entity.IsDeleted = request.CoreData.IsDeleted;
                entity.CanCarpentry = ParseBool(request.Professions, "CanCarpentry");
                entity.CanWeld = ParseBool(request.Professions, "CanWeld");
                entity.CanDesignShip = ParseBool(request.Professions, "CanDesignShip");
                entity.CanPaint = ParseBool(request.Professions, "CanPaint");
                entity.CanRig = ParseBool(request.Professions, "CanRig");
                entity.CanShipyard = ParseBool(request.Professions, "CanShipyard");

                var newProfessionFromRequest = request.PrimaryProfession;
                if (newProfessionFromRequest != null)
                {
                    var currentProfession = entity.EmployeeProfessions.FirstOrDefault(ep => ep.FireDate == null);
                    var newProfession = await _dbContext.Professions.FirstOrDefaultAsync(p => p.Column == newProfessionFromRequest.Column);

                    if (newProfession != null &&
                        (currentProfession == null || currentProfession.ProfessionId != newProfession.Id)
                    )
                    {
                        if (currentProfession != null)
                        {
                            currentProfession.FireDate = newProfessionFromRequest.HireDate.AddSeconds(-1);
                        }

                        var newEmpProf = new EmployeeProfession
                        {
                            EmployeeId = entity.Id,
                            ProfessionId = newProfession.Id,
                            HireDate = newProfessionFromRequest.HireDate,
                            FireDate = null
                        };
                        await _dbContext.EmployeeProfessions.AddAsync(newEmpProf);
                    }
                }

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

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            var professions = await _dbContext.Professions.ToListAsync();

            var employeeCounts = _dbContext.EmployeeProfessions
                .Where(x => x.FireDate == null || x.FireDate > DateTime.UtcNow)
                .GroupBy(x => x.Profession!.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Title, x => x.Count);
            employeeCounts.Add("Все", _dbContext.EmployeesShipbuilding.Count());

            var data = professions.Select(p => new ProfessionCountDTO
            {
                Id = p.Id,
                ProfessionTitle = p.Title,
                Count = employeeCounts.ContainsKey(p.Title) ? employeeCounts[p.Title] : 0
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

        #endregion Professions

        #region Resources

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

        public async Task<IEnumerable<ResourceForecastDTO>> GetResourceForecastAsync(int days)
        {
            // All resources
            var resourcesMap = await _dbContext.ResourcesShipbuilding
                .ToDictionaryAsync(r => r.Title, r => r);
            if (resourcesMap.Count == 0) return [];

            // All norms
            var norms = await _dbContext.ProfessionResourceNorms
                .Include(n => n.Profession)
                .Include(n => n.Resource)
                .Where(n => n.Resource != null && n.Profession != null)
                .Select(n => new
                {
                    ProfessionTitle = n.Profession!.Title,
                    ResourceTitle = n.Resource!.Title,
                    n.Hours,
                    n.QuantityProduced
                })
                .ToListAsync();
            if (norms.Count == 0) return [];

            // Employee counts
            var employeeCounts = _dbContext.EmployeeProfessions
                .Where(x => x.FireDate == null || x.FireDate > DateTime.UtcNow)
                .GroupBy(x => x.Profession!.Title)
                .ToDictionary(x => x.Key, x => x.Count());

            // Grouping norms by resource
            var normsByResource = norms.GroupBy(n => n.ResourceTitle)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Calculate forecast
            var forecast = new List<ResourceForecastDTO>();

            foreach (var resourceTitle in resourcesMap.Keys)
            {
                var resource = resourcesMap[resourceTitle];

                // Limit for resource-profession
                var producedAmount = 0m;
                if (normsByResource.ContainsKey(resourceTitle))
                {
                    var limits = new List<decimal>();

                    foreach (var norm in normsByResource[resourceTitle])
                    {
                        // How many hours we have for profession-product pair
                        var employeesCount = employeeCounts.ContainsKey(norm.ProfessionTitle) ? employeeCounts[norm.ProfessionTitle] : 0;
                        var productsForProfession = norms.Count(n => n.ProfessionTitle == norm.ProfessionTitle);
                        var totalHoursPerDay = employeesCount * WORK_HOURS_PER_DAY / productsForProfession;

                        // How many units we can produce
                        var portionsPerDay = (decimal)totalHoursPerDay / norm.Hours * norm.QuantityProduced;
                        limits.Add(portionsPerDay);
                    }

                    producedAmount = limits.Min() * days;
                }

                forecast.Add(new ResourceForecastDTO
                {
                    Company = ServiceName,
                    Resource = resourceTitle,
                    CurrentAmount = resource.Count,
                    Unit = resource.Unit,
                    Days = days,
                    ProducedAmount = producedAmount,
                    TotalAmount = resource.Count + producedAmount
                });
            }

            return forecast;
        }

        #endregion Resources

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

        private static bool ParseBool(Dictionary<string, bool> data, string key)
        {
            return data?.GetValueOrDefault(key) ?? false;
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

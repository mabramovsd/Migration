using Migration.Agro.Entities;
using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Migration.Contracts.DTO.Resources;
using System.Linq.Expressions;
using System.Diagnostics.Metrics;

namespace Migration.Agro.Services
{
    public class HRServiceAgro : ICompanyService
    {
        private const string ServiceName = "Agro";
        private const decimal WORK_HOURS_PER_DAY = 5;

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


        public async Task<IEnumerable<EmployeeAdditionalInfo>> GetFilteredEmployees(EmployeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Profession))
            {
                return await GetEmployeeListAsync();
            }

            //Filter by profession
            var professions = await _dbContext.Professions
                .Where(c => c.Title == filter.Profession)
                .Select(p => p.Column).ToListAsync();
            if (!professions.Any())
            {
                return new List<EmployeeAdditionalInfo>();
            }

            //Mapping
            return await _dbContext.EmployeesAgro
                .Where(emp =>
                    emp.HasTracktorLicense && professions.Contains("HasTracktorLicense"))
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
                    Id = request.CoreData.Id,
                    HasTracktorLicense = hasTracktorLicense
                });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add Agro employee: {ErrorMessage}", ex.Message);
            }

            return request.CoreData.Id;
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

        public async Task<IEnumerable<ProfessionCountDTO>> GetProfessionsStatsAsync()
        {
            var allEmployees = await _dbContext.EmployeesAgro
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            var professions = await _dbContext.Professions.ToListAsync();

            var data = professions.Select(p => new ProfessionCountDTO
            {
                Id = p.Id,
                ProfessionTitle = p.Title,
                Count = allEmployees.Count(e =>
                    (p.Column == "All") ||
                    (p.Column == "HasTracktorLicense" && e.HasTracktorLicense)
                )
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
            var resources = await _dbContext.ResourcesAgro
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

        public async Task<IEnumerable<ResourceForecastDTO>> GetResourceForecastAsync(int days)
        {
            // All resources
            var resourcesMap = await _dbContext.ResourcesAgro
                .ToDictionaryAsync(r => r.Title, r => r);
            if (resourcesMap.Count == 0) return [];

            // All norms
            var norms = await _dbContext.ProfessionResourceNorms
                .Include(n => n.Profession)
                .Include(n => n.Resource)
                .Where(n => n.Resource != null && n.Profession != null)
                .Select(n => new
                {
                    ProfessionColumn = n.Profession!.Column,
                    ResourceTitle = n.Resource!.Title,
                    n.Hours,
                    n.QuantityProduced
                })
                .ToListAsync();
            if (norms.Count == 0) return [];

            // Employee counts (ToDo: N+1 cycle)
            var professionColumns = norms.Select(n => n.ProfessionColumn).Distinct().ToList();
            var employeeCounts = new Dictionary<string, int>();
            foreach (var column in professionColumns)
            {
                Expression<Func<EmployeeAgro, bool>> predicate = column switch
                {
                    "HasTracktorLicense" => e => e.HasTracktorLicense,
                    "IsMilker" => e => e.IsMilker,
                    "IsCattleman" => e => e.IsCattleman,
                    "IsPoultryFarmer" => e => e.IsPoultryFarmer,
                    "IsMiller" => e => e.IsMiller,
                    "IsVegetableGrower" => e => e.IsVegetableGrower,
                    _ => e => false // Unknown column
                };

                var count = await _dbContext.EmployeesAgro
                    .Where(e => !e.IsDeleted)
                    .CountAsync(predicate);

                employeeCounts[column] = count;
            }

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
                        var employeesCount = employeeCounts[norm.ProfessionColumn];
                        var productsForProfession = norms.Count(n => n.ProfessionColumn == norm.ProfessionColumn);
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
    }
}

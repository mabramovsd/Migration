using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Agro.Entities;
using Migration.Agro.Services;
using Migration.Contracts.DTO.Employees;
using Moq;
using Xunit;

namespace Migration.Agro.Tests;

public class HRServiceTests : IDisposable
{
    private AgroDBContext _context;
    private HRServiceAgro _service;
    private Mock<ILogger<HRServiceAgro>> _loggerMock;

    public HRServiceTests()
    {
        // Use unique database and Dispose to make separate DB for each test (guaranteed)
        var dbName = "AgroTestDb" + Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AgroDBContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        _context = new AgroDBContext(options);
        _loggerMock = new Mock<ILogger<HRServiceAgro>>();
        _service = new HRServiceAgro(_context, _loggerMock.Object);
    }

    #region Mock data

    private static EmployeeAgro CreateEmployee(bool isDeleted, bool isVegetableGrower, bool hasTracktorLicense)
    {
        return new EmployeeAgro
        {
            Id = Guid.NewGuid(),
            IsDeleted = isDeleted,
            IsVegetableGrower = isVegetableGrower,
            HasTracktorLicense = hasTracktorLicense
        };
    }

    private static Profession CreateProfession(string title, string column)
    {
        return new Profession
        {
            Id = Guid.NewGuid(),
            Title = title,
            Column = column
        };
    }

    private static ResourceAgro CreateResource(string title, int count, string unit)
    {
        return new ResourceAgro
        {
            Id = Guid.NewGuid(),
            Title = title,
            Count = count,
            Unit = unit
        };
    }

    #endregion Mock data

    #region CUD for employee

    [Fact]
    public async Task AddEmployeeAsync_ShouldCreateEmployee()
    {
        var request = new CreateEmployeeRequest
        {
            CoreData = new Employee 
            { 
                Id = Guid.NewGuid(),
                FullName = "John",
                BirthDate = DateTime.UtcNow,
                CurrentCompany = "Agro"
            },
            Professions = new Dictionary<string, bool> 
            { 
                { 
                    "IsVegetableGrower", 
                    true
                }
            }
        };
        var id = await _service.AddEmployeeAsync(request);
        var saved = await _context.EmployeesAgro.FindAsync(id);
        Assert.True(saved != null && saved.IsVegetableGrower);
    }
    [Fact]
    public async Task UpdateEmployeeAsync_UpdatesFields()
    {
        // Arrange
        var employee = CreateEmployee(false, true, false);
        await _context.EmployeesAgro.AddAsync(employee);
        await _context.SaveChangesAsync();

        var request = new CreateEmployeeRequest
        {
            CoreData = new Employee
            {
                Id = employee.Id,
                FullName = "Updated Name",
                BirthDate = DateTime.UtcNow,
                CurrentCompany = "Agro",
                IsDeleted = false
            },
            Professions = new Dictionary<string, bool>
            {
                { "IsVegetableGrower", false },
                { "HasTracktorLicense", true }
            }
        };

        // Act
        var result = await _service.UpdateEmployeeAsync(request);

        // Assert
        Assert.Equal(employee.Id, result);
        var updated = await _context.EmployeesAgro.FindAsync(employee.Id);
        Assert.NotNull(updated);
        Assert.False(updated.IsVegetableGrower);
        Assert.True(updated.HasTracktorLicense);
        Assert.Equal("Updated Name", request.CoreData.FullName);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ReturnsEmpty_WhenNotFound()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            CoreData = new Employee { Id = Guid.NewGuid() },
            Professions = new Dictionary<string, bool>()
        };

        // Act
        var result = await _service.UpdateEmployeeAsync(request);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_SoftDelete_MarksDeleted()
    {
        // Arrange
        var employee = CreateEmployee(false, true, false);
        await _context.EmployeesAgro.AddAsync(employee);
        await _context.SaveChangesAsync();

        var request = new RemoveEmployeeRequest { Id = employee.Id, SoftDelete = true };

        // Act
        var success = await _service.RemoveEmployeeAsync(request);

        // Assert
        Assert.True(success);
        var deleted = await _context.EmployeesAgro.FindAsync(employee.Id);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_HardDelete_Removes()
    {
        // Arrange
        var employee = CreateEmployee(false, true, false);
        await _context.EmployeesAgro.AddAsync(employee);
        await _context.SaveChangesAsync();

        var request = new RemoveEmployeeRequest { Id = employee.Id, SoftDelete = false };

        // Act
        var success = await _service.RemoveEmployeeAsync(request);

        // Assert
        Assert.True(success);
        var deleted = await _context.EmployeesAgro.FindAsync(employee.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        var request = new RemoveEmployeeRequest { Id = Guid.NewGuid(), SoftDelete = true };

        // Act
        var success = await _service.RemoveEmployeeAsync(request);

        // Assert
        Assert.False(success);
    }


    #endregion CUD for employee

    #region Filtering for employee

    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenExists()
    {
        // Arrange
        var employee = CreateEmployee(false, true, false);
        await _context.EmployeesAgro.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEmployeeByIdAsync(employee.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.True((bool)result.Professions["IsVegetableGrower"]);
    }

    [Fact]
    public async Task GetFilteredEmployees_ReturnsFilteredResult()
    {
        // Arrange
        var profession = CreateProfession("Farmer", "IsVegetableGrower");
        await _context.Professions.AddAsync(profession);
        await _context.SaveChangesAsync();

        var employee1 = CreateEmployee(false, true, false);
        var employee2 = CreateEmployee(false, false, true);
        await _context.EmployeesAgro.AddRangeAsync(employee1, employee2);
        await _context.SaveChangesAsync();

        var filter = new EmployeeFilter
        {
            Company = "Agro",
            Profession = "Farmer"
        };

        // Act
        var result = await _service.GetFilteredEmployees(filter);

        // Assert
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal(employee1.Id, resultList[0].Id);
    }

    [Fact]
    public async Task GetEmployeeListAsync_ReturnsAllNonDeleted()
    {
        // Arrange
        var emp1 = CreateEmployee(false, true, false);
        var emp2 = CreateEmployee(false, false, true);
        var emp3 = CreateEmployee(true, false, false); // deleted
        await _context.EmployeesAgro.AddRangeAsync(emp1, emp2, emp3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEmployeeListAsync();

        // Assert
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Contains(list, e => e.Id == emp1.Id);
        Assert.Contains(list, e => e.Id == emp2.Id);
        Assert.DoesNotContain(list, e => e.Id == emp3.Id);
    }

    #endregion Filtering for employee

    #region Professions, Resources

    [Fact]
    public async Task GetProfessionsStats_ReturnsCorrectCounts()
    {
        // Arrange
        var profession = CreateProfession("Farmer", "IsVegetableGrower");
        await _context.Professions.AddAsync(profession);
        await _context.SaveChangesAsync();

        var employee1 = CreateEmployee(false, true, false);
        var employee2 = CreateEmployee(false, true, true);
        var employee3 = CreateEmployee(false, false, false);
        await _context.EmployeesAgro.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act
        var stats = await _service.GetProfessionsStatsAsync();
        var farmerStat = stats.FirstOrDefault(s => s.ProfessionTitle == "Farmer");

        // Assert
        Assert.NotNull(farmerStat);
        Assert.Equal(2, farmerStat.Count);
    }


    [Fact]
    public async Task GetResourcesAsync_ReturnsResources()
    {
        // Arrange
        var resource = CreateResource("Wheat", 100, "kg");
        await _context.ResourcesAgro.AddAsync(resource);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetResourcesAsync();

        // Assert
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal("Wheat", list[0].Title);
        Assert.Equal(100, list[0].Count);
    }

    [Fact]
    public async Task GetResourceForecastAsync_ReturnsForecast_WhenDataExists()
    {
        // Arrange
        var profession = CreateProfession("Farmer", "IsVegetableGrower");
        await _context.Professions.AddAsync(profession);

        var resource = CreateResource("Flour", 50, "kg");
        await _context.ResourcesAgro.AddAsync(resource);

        var norm = new ProfessionResourceNorm
        {
            Id = Guid.NewGuid(),
            ProfessionId = profession.Id,
            ResourceId = resource.Id,
            Hours = 2,
            QuantityProduced = 10
        };
        await _context.ProfessionResourceNorms.AddAsync(norm);

        var emp1 = CreateEmployee(false, true, false);
        var emp2 = CreateEmployee(false, true, false);
        await _context.EmployeesAgro.AddRangeAsync(emp1, emp2);
        await _context.SaveChangesAsync();

        // Act
        var forecast = await _service.GetResourceForecastAsync(3);

        // Assert
        var list = forecast.ToList();
        Assert.Single(list);
        var item = list[0];
        Assert.Equal("Flour", item.Resource);
        Assert.Equal(50, item.CurrentAmount);
        // Was 50kg. Each worker works 5 hours per day, 10 units each 2 hours
        // 5/2*10=25 units per day per one worker
        // 2 workers * 25 units * 3 days = 150. 150 + 50 (already on warehouse) = 200
        Assert.Equal(200, item.TotalAmount);
    }

    #endregion Professions, Resources

    #region Dispose

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion Dispose
}
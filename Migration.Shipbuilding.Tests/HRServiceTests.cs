using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Shipbuilding.Entities;
using Migration.Shipbuilding.Services;
using Migration.Contracts.DTO.Employees;
using Moq;
using Xunit;
using Migration.Contracts.DTO.Professions;

namespace Migration.Shipbuilding.Tests;

public class HRServiceShipbuildingTests : IDisposable
{
    private readonly ShipbuildingDBContext _context;
    private readonly HRServiceShipbuilding _service;
    private readonly Mock<ILogger<HRServiceShipbuilding>> _loggerMock;

    public HRServiceShipbuildingTests()
    {
        var dbName = "ShipbuildingTestDb_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<ShipbuildingDBContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        _context = new ShipbuildingDBContext(options);
        _loggerMock = new Mock<ILogger<HRServiceShipbuilding>>();
        _service = new HRServiceShipbuilding(_context, _loggerMock.Object);
    }

    #region Helpers

    private static EmployeeShipbuilding CreateEmployee(
        bool canCarpentry = false,
        bool canWeld = false,
        bool canDesignShip = false,
        bool canPaint = false,
        bool canRig = false,
        bool canShipyard = false,
        bool isDeleted = false)
    {
        return new EmployeeShipbuilding
        {
            Id = Guid.NewGuid(),
            CanCarpentry = canCarpentry,
            CanWeld = canWeld,
            CanDesignShip = canDesignShip,
            CanPaint = canPaint,
            CanRig = canRig,
            CanShipyard = canShipyard,
            IsDeleted = isDeleted
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

    private static EmployeeProfession CreateEmployeeProfession(Guid employeeId, Guid professionId, DateTime? fireDate = null)
    {
        return new EmployeeProfession
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ProfessionId = professionId,
            HireDate = DateTime.UtcNow.AddDays(-5),
            FireDate = fireDate
        };
    }

    #endregion

    [Fact]
    public async Task GetFilteredEmployees_ReturnsFilteredResult()
    {
        // Arrange
        var profession = CreateProfession("Carpenter", "CanCarpentry");
        await _context.Professions.AddAsync(profession);
        await _context.SaveChangesAsync();

        var employee1 = CreateEmployee(canCarpentry: true);
        var employee2 = CreateEmployee(canWeld: true);
        await _context.EmployeesShipbuilding.AddRangeAsync(employee1, employee2);
        await _context.SaveChangesAsync();

        var empProf = CreateEmployeeProfession(employee1.Id, profession.Id);
        await _context.EmployeeProfessions.AddAsync(empProf);
        await _context.SaveChangesAsync();

        var filter = new EmployeeFilter
        {
            Company = "Shipbuilding",
            Profession = "Carpenter"
        };

        // Act
        var result = await _service.GetFilteredEmployees(filter);

        // Assert
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal(employee1.Id, resultList[0].Id);
    }

    [Fact]
    public async Task GetProfessionsStats_ReturnsCorrectCounts()
    {
        // Arrange
        var profession1 = CreateProfession("Carpenter", "CanCarpentry");
        var profession2 = CreateProfession("Welder", "CanWeld");
        await _context.Professions.AddRangeAsync(profession1, profession2);
        await _context.SaveChangesAsync();

        var employee1 = CreateEmployee(canCarpentry: true);
        var employee2 = CreateEmployee(canCarpentry: true);
        var employee3 = CreateEmployee(canWeld: true);
        await _context.EmployeesShipbuilding.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        var empProf1 = CreateEmployeeProfession(employee1.Id, profession1.Id);
        var empProf2 = CreateEmployeeProfession(employee2.Id, profession1.Id);
        await _context.EmployeeProfessions.AddRangeAsync(empProf1, empProf2);
        await _context.SaveChangesAsync();

        // Act
        var stats = await _service.GetProfessionsStatsAsync();
        var carpenterStat = stats.FirstOrDefault(s => s.ProfessionTitle == "Carpenter");

        // Assert
        Assert.NotNull(carpenterStat);
        Assert.Equal(2, carpenterStat.Count);
    }

    #region Professions, Resources

    [Fact]
    public async Task AddEmployeeAsync_WithPrimaryProfession_CreatesEmployeeProfession()
    {
        // Arrange
        var profession = CreateProfession("Carpenter", "CanCarpentry");
        await _context.Professions.AddAsync(profession);
        await _context.SaveChangesAsync();

        var hireDate = new DateTime(2026, 01, 15);
        var request = new CreateEmployeeRequest
        {
            CoreData = new Employee
            {
                Id = Guid.NewGuid(),
                FullName = "Test",
                BirthDate = DateTime.UtcNow,
                CurrentCompany = "Shipbuilding"
            },
            Professions = new Dictionary<string, bool> 
            { 
                { 
                    "CanCarpentry", 
                    true 
                } 
            },
            PrimaryProfession = new PrimaryProfession
            {
                Column = "CanCarpentry",
                HireDate = hireDate,
                FireDate = null
            }
        };

        // Act
        var result = await _service.AddEmployeeAsync(request);

        // Assert
        var empProf = await _context.EmployeeProfessions
            .FirstOrDefaultAsync(ep => ep.EmployeeId == result);
        Assert.NotNull(empProf);
        Assert.Equal(profession.Id, empProf.ProfessionId);
        Assert.Equal(hireDate, empProf.HireDate);
        Assert.Null(empProf.FireDate);
    }

    [Fact]
    public async Task GetResourcesAsync_ReturnsListOfResources()
    {
        // Arrange
        var resource = new ResourceShipbuilding
        {
            Id = Guid.NewGuid(),
            Title = "Steel",
            Count = 100,
            Unit = "kg"
        };
        await _context.ResourcesShipbuilding.AddAsync(resource);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetResourcesAsync();
        var list = result.ToList();

        // Assert
        Assert.Single(list);
        Assert.Equal("Steel", list[0].Title);
        Assert.Equal(100, list[0].Count);
        Assert.Equal("kg", list[0].Unit);
    }

    [Fact]
    public async Task GetProfessionResourceNormsAsync_ReturnsNorms()
    {
        // Arrange
        var profession = CreateProfession("Carpenter", "CanCarpentry");
        var resource = new ResourceShipbuilding
        {
            Id = Guid.NewGuid(),
            Title = "Wood",
            Count = 200,
            Unit = "m3"
        };
        await _context.Professions.AddAsync(profession);
        await _context.ResourcesShipbuilding.AddAsync(resource);
        await _context.SaveChangesAsync();

        var norm = new ProfessionResourceNorm
        {
            Id = Guid.NewGuid(),
            ProfessionId = profession.Id,
            ResourceId = resource.Id,
            Hours = 2,
            QuantityProduced = 10
        };
        await _context.ProfessionResourceNorms.AddAsync(norm);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetProfessionResourceNormsAsync();
        var list = result.ToList();

        // Assert
        Assert.Single(list);
        Assert.Equal("Carpenter", list[0].Profession);
        Assert.Equal("Wood", list[0].Resource);
        Assert.Equal(2, list[0].Hours);
        Assert.Equal(10, list[0].QuantityProduced);
    }

    #endregion Professions, Resources

    public void Dispose()
    {
        _context.Dispose();
    }
}
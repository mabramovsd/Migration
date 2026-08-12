using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Shipbuilding.Entities;
using Migration.Shipbuilding.Services;
using Migration.Contracts.DTO.Employees;
using Moq;
using Xunit;

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

    public void Dispose()
    {
        _context.Dispose();
    }
}
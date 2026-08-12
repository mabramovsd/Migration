using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Agro.Entities;
using Migration.Agro.Services;
using Migration.Contracts.DTO.Employees;
using Migration.Contracts.DTO.Professions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Migration.Agro.Tests;

public class HRServiceAgroTests : IDisposable
{
    private AgroDBContext _context;
    private HRServiceAgro _service;
    private Mock<ILogger<HRServiceAgro>> _loggerMock;

    public HRServiceAgroTests()
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

    #endregion Mock data

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

    #region Dispose

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion Dispose
}
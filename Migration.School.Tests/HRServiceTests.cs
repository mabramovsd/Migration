using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Contracts.DTO.Employees;
using Migration.School.Services;
using Moq;
using Xunit;

namespace Migration.School.Tests;

public class HRServiceSchoolTests : IDisposable
{
    private readonly SchoolDBContext _context;
    private readonly HRServiceSchool _service;
    private readonly Mock<ILogger<HRServiceSchool>> _loggerMock;

    public HRServiceSchoolTests()
    {
        var dbName = "SchoolTestDb_" + Guid.NewGuid();
        var options = new DbContextOptionsBuilder<SchoolDBContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        _context = new SchoolDBContext(options);
        _loggerMock = new Mock<ILogger<HRServiceSchool>>();
        _service = new HRServiceSchool(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region Helpers

    private CreateEmployeeRequest CreateRequest(Guid? id = null)
    {
        return new CreateEmployeeRequest
        {
            CoreData = new Employee
            {
                Id = id ?? Guid.NewGuid(),
                FullName = "Test Employee",
                BirthDate = DateTime.UtcNow,
                CurrentCompany = "School",
                IsDeleted = false
            }
        };
    }

    #endregion

    [Fact]
    public async Task AddEmployeeAsync_ShouldAddEmployee()
    {
        // Arrange
        var request = CreateRequest();

        // Act
        var result = await _service.AddEmployeeAsync(request);

        // Assert
        Assert.Equal(request.CoreData.Id, result);
        var saved = await _context.EmployeesSchool.FindAsync(result);
        Assert.NotNull(saved);
        Assert.Equal(request.CoreData.Id, saved.Id);
        Assert.False(saved.IsDeleted);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenExists()
    {
        // Arrange
        var request = CreateRequest();
        await _service.AddEmployeeAsync(request);

        // Act
        var result = await _service.GetEmployeeByIdAsync(request.CoreData.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.CoreData.Id, result.Id);
        Assert.Null(result.Professions);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _service.GetEmployeeByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsNull_WhenDeleted()
    {
        // Arrange
        var request = CreateRequest();
        await _service.AddEmployeeAsync(request);

        // Soft delete
        var removeRequest = new RemoveEmployeeRequest { Id = request.CoreData.Id, SoftDelete = true };
        await _service.RemoveEmployeeAsync(removeRequest);

        // Act
        var result = await _service.GetEmployeeByIdAsync(request.CoreData.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEmployeeListAsync_ReturnsOnlyNonDeleted()
    {
        // Arrange
        var emp1 = CreateRequest();
        var emp2 = CreateRequest();
        var emp3 = CreateRequest();

        await _service.AddEmployeeAsync(emp1);
        await _service.AddEmployeeAsync(emp2);
        await _service.AddEmployeeAsync(emp3);

        // Soft delete
        await _service.RemoveEmployeeAsync(new RemoveEmployeeRequest { Id = emp2.CoreData.Id, SoftDelete = true });

        // Act
        var result = await _service.GetEmployeeListAsync();
        var list = result.ToList();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Contains(list, e => e.Id == emp1.CoreData.Id);
        Assert.Contains(list, e => e.Id == emp3.CoreData.Id);
        Assert.DoesNotContain(list, e => e.Id == emp2.CoreData.Id);
    }

    [Fact]
    public async Task GetFilteredEmployees_ReturnsAllNonDeleted()
    {
        // Arrange
        var emp1 = CreateRequest();
        var emp2 = CreateRequest();
        await _service.AddEmployeeAsync(emp1);
        await _service.AddEmployeeAsync(emp2);
        await _service.RemoveEmployeeAsync(new RemoveEmployeeRequest { Id = emp2.CoreData.Id, SoftDelete = true });

        var filter = new EmployeeFilter { Company = "School" }; // Company не используется

        // Act
        var result = await _service.GetFilteredEmployees(filter);
        var list = result.ToList();

        // Assert
        Assert.Single(list);
        Assert.Equal(emp1.CoreData.Id, list[0].Id);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateIsDeleted()
    {
        // Arrange
        var request = CreateRequest();
        await _service.AddEmployeeAsync(request);

        var updateRequest = new CreateEmployeeRequest
        {
            CoreData = new Employee
            {
                Id = request.CoreData.Id,
                FullName = "Updated Name",
                BirthDate = DateTime.UtcNow,
                CurrentCompany = "School",
                IsDeleted = true
            }
        };

        // Act
        var result = await _service.UpdateEmployeeAsync(updateRequest);

        // Assert
        Assert.Equal(request.CoreData.Id, result);
        var updated = await _context.EmployeesSchool.FindAsync(request.CoreData.Id);
        Assert.True(updated.IsDeleted);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_SoftDelete_MarksDeleted()
    {
        // Arrange
        var request = CreateRequest();
        await _service.AddEmployeeAsync(request);

        var removeRequest = new RemoveEmployeeRequest { Id = request.CoreData.Id, SoftDelete = true };

        // Act
        var success = await _service.RemoveEmployeeAsync(removeRequest);

        // Assert
        Assert.True(success);
        var entity = await _context.EmployeesSchool.FindAsync(request.CoreData.Id);
        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_HardDelete_Removes()
    {
        // Arrange
        var request = CreateRequest();
        await _service.AddEmployeeAsync(request);

        var removeRequest = new RemoveEmployeeRequest { Id = request.CoreData.Id, SoftDelete = false };

        // Act
        var success = await _service.RemoveEmployeeAsync(removeRequest);

        // Assert
        Assert.True(success);
        var entity = await _context.EmployeesSchool.FindAsync(request.CoreData.Id);
        Assert.Null(entity);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        var removeRequest = new RemoveEmployeeRequest { Id = Guid.NewGuid(), SoftDelete = true };

        // Act
        var success = await _service.RemoveEmployeeAsync(removeRequest);

        // Assert
        Assert.False(success);
    }

    #region Professions, Resources

    [Fact]
    public async Task GetProfessionsStatsAsync_ReturnsEmpty()
    {
        var result = await _service.GetProfessionsStatsAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProfessionsAsync_ReturnsEmpty()
    {
        var result = await _service.GetProfessionsAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProfessionResourceNormsAsync_ReturnsEmpty()
    {
        var result = await _service.GetProfessionResourceNormsAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetResourcesAsync_ReturnsEmpty()
    {
        var result = await _service.GetResourcesAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetResourceForecastAsync_ReturnsEmpty()
    {
        var result = await _service.GetResourceForecastAsync(5);
        Assert.Empty(result);
    }

    #endregion Professions, Resources
}
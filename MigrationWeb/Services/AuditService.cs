using System.Security.Principal;
using System.Text.Json;
using Migration.Contracts;
using Migration.Contracts.DTO;

namespace MigrationWeb.Services;

public class AuditService : IAuditService
{
    private readonly CoreDBContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(CoreDBContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Auxillary methods

    private string? GetUserName()
    {
        // In future it will work and we'll can get user from HTTP-context or JWT
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return httpContext.User.Identity.Name
                ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? "anonymous";
        }

        //But now - this one))
        return "system";
    }

    private static string Serialize(object? obj)
    {
        if (obj == null) return string.Empty;
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    #endregion Auxillary methods

    private async Task LogOperationAsync(Guid entityId, string entityType, AuditOperation operation, string? userName, object? oldEntity, object? newEntity)
    {
        await _context.AuditRecords.AddAsync(new AuditRecord
        {
            Id = Guid.NewGuid(),
            EntityId = entityId,
            EntityType = entityType,
            Operation = nameof(operation),
            UserName = userName ?? GetUserName(),
            Timestamp = DateTime.UtcNow,
            OldValues = (operation == AuditOperation.Create) ? null : Serialize(oldEntity),
            NewValues = (operation == AuditOperation.Delete) ? null : Serialize(newEntity)
        });
        await _context.SaveChangesAsync();
    }

    public async Task LogCreateAsync(Guid entityId, string entityType, string? userName, object? entity) => 
        await LogOperationAsync(entityId, entityType, AuditOperation.Create, userName, null, entity);

    public async Task LogUpdateAsync(Guid entityId, string entityType, string? userName, object? oldEntity, object? newEntity) =>
        await LogOperationAsync(entityId, entityType, AuditOperation.Update, userName, oldEntity, newEntity);

    public async Task LogDeleteAsync(Guid entityId, string entityType, string? userName, object? entity) =>
        await LogOperationAsync(entityId, entityType, AuditOperation.Delete, userName, entity, null);
}

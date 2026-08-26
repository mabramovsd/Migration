namespace MigrationWeb.Services;

public interface IAuditService
{
    Task LogCreateAsync(Guid entityId, string entityType, string? userName, object? entity);
    Task LogUpdateAsync(Guid entityId, string entityType, string? userName, object? oldEntity, object? newEntity);
    Task LogDeleteAsync(Guid entityId, string entityType, string? userName, object? entity);
}

namespace RealStatePortal.Application.Auditing;

public interface IAuditService
{
    Task RecordAsync(
        string entityName,
        Guid entityId,
        string action,
        string? details = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditLogDto>> GetByEntityAsync(
        string entityName,
        Guid entityId,
        CancellationToken cancellationToken = default);
}
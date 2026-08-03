using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IAuditLogRepository
{
    Task<IReadOnlyCollection<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default);
    void Add(AuditLog auditLog);
}
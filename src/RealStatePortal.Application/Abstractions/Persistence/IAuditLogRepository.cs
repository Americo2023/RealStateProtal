using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
}

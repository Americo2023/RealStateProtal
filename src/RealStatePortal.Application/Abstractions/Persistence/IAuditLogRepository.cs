using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IAuditLogRepository
{
    Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
}

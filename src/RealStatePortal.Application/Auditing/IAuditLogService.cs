using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.Auditing;

public interface IAuditLogService
{
    Task<Result<IReadOnlyCollection<AuditLogDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}

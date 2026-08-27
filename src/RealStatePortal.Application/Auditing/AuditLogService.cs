using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.Auditing;

public sealed class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    public async Task<Result<IReadOnlyCollection<AuditLogDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var logs = await auditLogRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<AuditLogDto>>.Success(logs.Select(log => new AuditLogDto(
            log.Id,
            log.EntityName,
            log.EntityId,
            log.Action,
            log.ChangedByUserId,
            log.ChangedAt,
            log.Details)).ToArray());
    }
}

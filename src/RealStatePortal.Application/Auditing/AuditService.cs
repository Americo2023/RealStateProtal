using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Auditing;

public sealed class AuditService(
    IAuditLogRepository auditRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IAuditService
{
    public async Task RecordAsync(
        string entityName,
        Guid entityId,
        string action,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            entityName,
            entityId,
            action,
            currentUser.UserId,
            dateTimeProvider.UtcNow,
            details);
        auditRepository.Add(auditLog);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditLogDto>> GetByEntityAsync(
        string entityName,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInRole(UserRole.Administrator))
        {
            throw new UnauthorizedAccessException("Only administrators can view audit logs.");
        }

        var logs = await auditRepository.GetByEntityAsync(entityName, entityId, cancellationToken);
        return logs.Select(log => new AuditLogDto(
            log.Id,
            log.EntityName,
            log.EntityId,
            log.Action,
            log.ChangedByUserId,
            log.ChangedAt,
            log.Details)).ToArray();
    }
}
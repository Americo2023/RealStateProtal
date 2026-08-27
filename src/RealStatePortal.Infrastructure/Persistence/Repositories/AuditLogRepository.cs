using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(RealStatePortalDbContext dbContext) : IAuditLogRepository
{
    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default) =>
        dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public async Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.ChangedAt)
            .ToArrayAsync(cancellationToken);
}
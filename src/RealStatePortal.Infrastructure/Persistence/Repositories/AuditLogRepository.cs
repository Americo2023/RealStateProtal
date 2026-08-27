using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(RealStatePortalDbContext dbContext) : IAuditLogRepository
{
    public async Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.ChangedAt)
            .ToArrayAsync(cancellationToken);
}
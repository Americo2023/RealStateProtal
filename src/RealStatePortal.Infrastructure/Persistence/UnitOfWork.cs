using RealStatePortal.Application.Abstractions.Persistence;

namespace RealStatePortal.Infrastructure.Persistence;

public sealed class UnitOfWork(RealStatePortalDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
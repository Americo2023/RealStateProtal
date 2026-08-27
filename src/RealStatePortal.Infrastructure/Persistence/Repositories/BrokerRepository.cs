using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository(RealStatePortalDbContext dbContext) : IBrokerRepository
{
    public async Task<IReadOnlyCollection<BrokerProfile>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.BrokerProfiles
            .AsNoTracking()
            .OrderBy(broker => broker.FullName)
            .ToArrayAsync(cancellationToken);

    public Task<BrokerProfile?> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default) =>
        dbContext.BrokerProfiles.SingleOrDefaultAsync(broker => broker.Id == brokerId, cancellationToken);

    public Task<BrokerProfile?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default) =>
        dbContext.Properties
            .Where(property => property.Id == propertyId)
            .Join(dbContext.BrokerProfiles, property => property.BrokerId, broker => broker.UserId, (_, broker) => broker)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
}
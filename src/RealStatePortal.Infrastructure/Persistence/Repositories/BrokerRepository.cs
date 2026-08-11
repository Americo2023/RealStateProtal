using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository(RealStatePortalDbContext dbContext) : IBrokerRepository
{
    public Task<BrokerProfile?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default) =>
        dbContext.Properties
            .Where(property => property.Id == propertyId)
            .Join(dbContext.BrokerProfiles, property => property.BrokerId, broker => broker.UserId, (_, broker) => broker)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
}
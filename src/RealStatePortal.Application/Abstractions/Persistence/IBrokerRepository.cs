using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IBrokerRepository
{
    Task<BrokerProfile?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
}
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IBrokerRepository
{
    Task<IReadOnlyCollection<BrokerProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BrokerProfile?> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
    Task<BrokerProfile?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
}
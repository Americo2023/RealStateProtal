using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IBrokerProfileRepository
{
    Task<BrokerProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BrokerProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(BrokerProfile brokerProfile);
}
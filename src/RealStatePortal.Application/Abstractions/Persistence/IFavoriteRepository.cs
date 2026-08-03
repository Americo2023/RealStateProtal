using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IFavoriteRepository
{
    Task<IReadOnlyCollection<Favorite>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<Favorite?> GetAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    void Add(Favorite favorite);
    void Remove(Favorite favorite);
}
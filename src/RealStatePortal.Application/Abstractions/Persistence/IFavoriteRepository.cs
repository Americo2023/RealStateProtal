using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IFavoriteRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Favorite>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
}
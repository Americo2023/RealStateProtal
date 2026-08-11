using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class FavoriteRepository(RealStatePortalDbContext dbContext) : IFavoriteRepository
{
    public Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default) =>
        dbContext.Favorites.AnyAsync(favorite => favorite.UserId == userId && favorite.PropertyId == propertyId, cancellationToken);

    public async Task<IReadOnlyCollection<Favorite>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.Favorites.AsNoTracking().Where(favorite => favorite.UserId == userId).OrderByDescending(favorite => favorite.CreatedAt).ToArrayAsync(cancellationToken);

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default) =>
        await dbContext.Favorites.AddAsync(favorite, cancellationToken);

    public async Task RemoveAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var favorite = await dbContext.Favorites.SingleOrDefaultAsync(
            candidate => candidate.UserId == userId && candidate.PropertyId == propertyId,
            cancellationToken);

        if (favorite is not null)
        {
            dbContext.Favorites.Remove(favorite);
        }
    }
}
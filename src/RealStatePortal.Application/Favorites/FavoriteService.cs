using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Favorites;

public sealed class FavoriteService(
    IFavoriteRepository favoriteRepository,
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IFavoriteService
{
    public async Task<FavoriteDto> AddAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var userId = RequireUserId();
        var property = await propertyRepository.GetByIdAsync(propertyId, cancellationToken)
            ?? throw new KeyNotFoundException($"Property '{propertyId}' was not found.");

        if (property.Status != PropertyStatus.Published)
        {
            throw new InvalidOperationException("Only published properties can be added to favorites.");
        }

        if (await favoriteRepository.ExistsAsync(userId, propertyId, cancellationToken))
        {
            throw new InvalidOperationException("The property is already a favorite.");
        }

        var favorite = new Favorite(Guid.NewGuid(), userId, propertyId, dateTimeProvider.UtcNow);
        favoriteRepository.Add(favorite);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(favorite);
    }

    public async Task<IReadOnlyCollection<FavoriteDto>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var favorites = await favoriteRepository.GetByUserIdAsync(RequireUserId(), cancellationToken);
        return favorites.Select(Map).ToArray();
    }

    public async Task RemoveAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var favorite = await favoriteRepository.GetAsync(RequireUserId(), propertyId, cancellationToken)
            ?? throw new KeyNotFoundException("The favorite was not found.");
        favoriteRepository.Remove(favorite);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Guid RequireUserId() => currentUser.UserId
        ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    private static FavoriteDto Map(Favorite favorite) =>
        new(favorite.UserId, favorite.PropertyId, favorite.CreatedAt);
}
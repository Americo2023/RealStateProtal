using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Common;
using RealStatePortal.Application.Properties.Services;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Favorites;

public sealed class FavoriteService(
    IFavoriteRepository favoriteRepository,
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IPropertyService propertyService) : IFavoriteService
{
    public async Task<Result> AddAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Result.Failure("Authentication is required.");
        }

        var property = await propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null || property.Status != Domain.Enums.PropertyStatus.Published)
        {
            return Result.Failure("Only published properties can be added to favorites.");
        }

        if (await favoriteRepository.ExistsAsync(currentUser.UserId.Value, propertyId, cancellationToken))
        {
            return Result.Failure("The property is already a favorite.");
        }

        await favoriteRepository.AddAsync(new Favorite(currentUser.UserId.Value, propertyId, dateTimeProvider.UtcNow), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Result.Failure("Authentication is required.");
        }

        if (!await favoriteRepository.ExistsAsync(currentUser.UserId.Value, propertyId, cancellationToken))
        {
            return Result.Failure("The property is not a favorite.");
        }

        await favoriteRepository.RemoveAsync(currentUser.UserId.Value, propertyId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<FavoriteDto>>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Result<IReadOnlyCollection<FavoriteDto>>.Failure("Authentication is required.");
        }

        var favorites = await favoriteRepository.GetByUserIdAsync(currentUser.UserId.Value, cancellationToken);
        var result = new List<FavoriteDto>(favorites.Count);
        foreach (var favorite in favorites)
        {
            var propertyResult = await propertyService.GetByIdAsync(favorite.PropertyId, cancellationToken);
            if (propertyResult.IsSuccess)
            {
                result.Add(new FavoriteDto(favorite.Id, favorite.CreatedAt, propertyResult.Value!));
            }
        }

        return Result<IReadOnlyCollection<FavoriteDto>>.Success(result);
    }
}
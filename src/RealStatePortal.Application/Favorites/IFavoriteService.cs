namespace RealStatePortal.Application.Favorites;

public interface IFavoriteService
{
    Task<FavoriteDto> AddAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<FavoriteDto>> GetMineAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid propertyId, CancellationToken cancellationToken = default);
}
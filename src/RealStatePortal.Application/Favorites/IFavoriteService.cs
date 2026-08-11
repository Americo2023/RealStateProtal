using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.Favorites;

public interface IFavoriteService
{
    Task<Result> AddAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result> RemoveAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<FavoriteDto>>> GetMineAsync(CancellationToken cancellationToken = default);
}
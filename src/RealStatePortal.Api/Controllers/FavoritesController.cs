using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Favorites;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/favorites")]
public sealed class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<FavoriteDto>> GetMine(CancellationToken cancellationToken) =>
        favoriteService.GetMineAsync(cancellationToken);

    [HttpPost("{propertyId:guid}")]
    public Task<FavoriteDto> Add(Guid propertyId, CancellationToken cancellationToken) =>
        favoriteService.AddAsync(propertyId, cancellationToken);

    [HttpDelete("{propertyId:guid}")]
    public async Task<IActionResult> Remove(Guid propertyId, CancellationToken cancellationToken)
    {
        await favoriteService.RemoveAsync(propertyId, cancellationToken);
        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Favorites;

namespace RealStatePortal.Api.Controllers;

[Route("api/favorites")]
[Authorize(Policy = "RegisteredUser")]
public sealed class FavoritesController(IFavoriteService favoriteService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await favoriteService.GetMineAsync(cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{propertyId:guid}")]
    public async Task<IActionResult> Add(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.AddAsync(propertyId, cancellationToken);
        return FromResult(result);
    }

    [HttpDelete("{propertyId:guid}")]
    public async Task<IActionResult> Remove(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.RemoveAsync(propertyId, cancellationToken);
        return FromResult(result);
    }
}
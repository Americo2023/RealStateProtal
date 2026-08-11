using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RealStatePortal.Application.Abstractions.Authentication;

namespace RealStatePortal.Infrastructure.Authentication;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid? UserId => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
        ? userId
        : null;

    public string? Auth0UserId => User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles => User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
}
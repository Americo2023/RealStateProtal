using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Infrastructure.Authentication;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid? UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("user_id");
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public bool IsInRole(UserRole role) => User.IsInRole(role.ToString()) ||
        User.FindAll("role").Any(claim => string.Equals(claim.Value, role.ToString(), StringComparison.OrdinalIgnoreCase));
}
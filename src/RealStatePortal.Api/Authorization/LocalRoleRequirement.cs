using Microsoft.AspNetCore.Authorization;

namespace RealStatePortal.Api.Authorization;

public sealed class LocalRoleRequirement(params string[] allowedRoles) : IAuthorizationRequirement
{
    public IReadOnlyCollection<string> AllowedRoles { get; } = allowedRoles;
}
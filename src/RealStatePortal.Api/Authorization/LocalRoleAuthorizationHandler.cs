using Microsoft.AspNetCore.Authorization;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Api.Authorization;

public sealed class LocalRoleAuthorizationHandler(
    ICurrentUserService currentUser,
    IUserRepository userRepository) : AuthorizationHandler<LocalRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        LocalRoleRequirement requirement)
    {
        var user = currentUser.UserId is Guid userId
            ? await userRepository.GetByIdAsync(userId)
            : currentUser.Auth0UserId is not null
                ? await userRepository.GetByExternalIdAsync(currentUser.Auth0UserId)
                : null;

        if (user is null)
        {
            return;
        }

        var roles = await userRepository.GetRolesAsync(user.Id);
        var roleNames = roles.Select(role => role switch
        {
            UserRole.Broker => "Broker",
            UserRole.Administrator => "Administrator",
            UserRole.RegisteredUser => "Registered User",
            UserRole.Visitor => "Visitor",
            _ => role.ToString()
        });

        if (roleNames.Any(role => requirement.AllowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }
    }
}
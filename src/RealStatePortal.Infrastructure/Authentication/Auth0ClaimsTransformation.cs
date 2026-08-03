using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using RealStatePortal.Application.Abstractions.Authentication;

namespace RealStatePortal.Infrastructure.Authentication;

public sealed class Auth0ClaimsTransformation(IIdentityProvisioningService identityProvisioningService) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var auth0UserId = principal.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(auth0UserId) ||
            principal.HasClaim(claim => claim.Type == "internal_user_id"))
        {
            return principal;
        }

        var provisionedIdentity = await identityProvisioningService.ProvisionAsync(
            new Auth0Identity(
                auth0UserId,
                principal.FindFirstValue("email") ?? string.Empty,
                principal.FindFirstValue("given_name") ?? string.Empty,
                principal.FindFirstValue("family_name") ?? string.Empty));
        if (provisionedIdentity is null)
        {
            return principal;
        }

        var identity = new ClaimsIdentity("InternalUser");
        identity.AddClaim(new Claim("internal_user_id", provisionedIdentity.UserId.ToString()));
        identity.AddClaim(new Claim("realstateportal_role", provisionedIdentity.Role));
        principal.AddIdentity(identity);
        return principal;
    }
}
namespace RealStatePortal.Application.Abstractions.Authentication;

public interface IIdentityProvisioningService
{
    Task<ProvisionedIdentity?> ProvisionAsync(
        Auth0Identity identity,
        CancellationToken cancellationToken = default);
}

public sealed record Auth0Identity(
    string Subject,
    string Email,
    string FirstName,
    string LastName);

public sealed record ProvisionedIdentity(
    Guid UserId,
    string Role,
    bool IsActive);
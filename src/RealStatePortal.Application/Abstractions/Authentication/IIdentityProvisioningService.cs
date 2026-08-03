namespace RealStatePortal.Application.Abstractions.Authentication;

public interface IIdentityProvisioningService
{
    Task ProvisionAsync(Guid userId, string auth0UserId, CancellationToken cancellationToken = default);
}
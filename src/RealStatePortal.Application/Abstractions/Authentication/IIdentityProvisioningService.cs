namespace RealStatePortal.Application.Abstractions.Authentication;

public interface IIdentityProvisioningService
{
    Task ProvisionAsync(Guid userId, CancellationToken cancellationToken = default);
}
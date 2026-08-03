using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Authentication;

public sealed class IdentityProvisioningService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IIdentityProvisioningService
{
    public async Task<ProvisionedIdentity?> ProvisionAsync(
        Auth0Identity identity,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByAuth0UserIdAsync(identity.Subject, cancellationToken);
        if (existingUser is not null)
        {
            return existingUser.IsActive
                ? new ProvisionedIdentity(existingUser.Id, existingUser.Role.ToString(), true)
                : null;
        }

        var user = new User(
            Guid.NewGuid(),
            identity.Subject,
            identity.Email,
            identity.FirstName,
            identity.LastName,
            dateTimeProvider.UtcNow);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProvisionedIdentity(user.Id, user.Role.ToString(), user.IsActive);
    }
}
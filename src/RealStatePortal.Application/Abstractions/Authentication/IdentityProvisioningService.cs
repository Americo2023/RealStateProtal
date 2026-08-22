using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Authentication;

public sealed class IdentityProvisioningService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IIdentityProvisioningService
{
    public async Task<User> ProvisionAsync(ExternalUserProfile profile, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExternalIdAsync(profile.ExternalId, cancellationToken);
        if (user is null)
        {
            user = new User(profile.ExternalId, profile.Email, profile.FirstName, profile.LastName);
            await userRepository.AddAsync(user, cancellationToken);
            await userRepository.ReplaceRolesAsync(user.Id, profile.Roles, cancellationToken);
        }
        else
        {
            user.UpdateProfile(profile.Email, profile.FirstName, profile.LastName, dateTimeProvider.UtcNow);
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("The internal user is inactive.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user;
    }
}
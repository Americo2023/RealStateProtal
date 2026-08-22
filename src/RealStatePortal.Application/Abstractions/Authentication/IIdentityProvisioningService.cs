using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Abstractions.Authentication;

public interface IIdentityProvisioningService
{
    Task<User> ProvisionAsync(ExternalUserProfile profile, CancellationToken cancellationToken = default);
}

public sealed record ExternalUserProfile(
    string ExternalId,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<UserRole> Roles);
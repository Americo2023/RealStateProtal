using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Auditing;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Users;

public sealed class UserAdministrationService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditService auditService) : IUserAdministrationService
{
    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        if (await userRepository.GetByAuth0UserIdAsync(request.Auth0UserId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("A user with this Auth0 identity already exists.");
        }

        var user = new User(
            Guid.NewGuid(),
            request.Auth0UserId,
            request.Email,
            request.FirstName,
            request.LastName,
            dateTimeProvider.UtcNow);
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditService.RecordAsync("User", user.Id, "Created", cancellationToken: cancellationToken);
        return Map(user);
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        return Map(await GetRequiredAsync(id, cancellationToken));
    }

    public async Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToArray();
    }

    public async Task ChangeRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var user = await GetRequiredAsync(id, cancellationToken);
        user.ChangeRole(role, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditService.RecordAsync("User", id, "RoleChanged", role.ToString(), cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var user = await GetRequiredAsync(id, cancellationToken);
        user.Deactivate(dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditService.RecordAsync("User", id, "Deactivated", cancellationToken: cancellationToken);
    }

    private async Task<User> GetRequiredAsync(Guid id, CancellationToken cancellationToken) =>
        await userRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new KeyNotFoundException($"User '{id}' was not found.");

    private void EnsureAdministrator()
    {
        if (!currentUser.IsInRole(UserRole.Administrator))
        {
            throw new UnauthorizedAccessException("Only administrators can manage users.");
        }
    }

    private static UserDto Map(User user) => new(
        user.Id,
        user.Auth0UserId,
        user.Email,
        user.FirstName,
        user.LastName,
        user.IsActive,
        user.Role,
        user.CreatedAt,
        user.UpdatedAt);
}
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Common;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Users;

public sealed class UserService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IUserService
{
    public async Task<Result<IReadOnlyCollection<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        var results = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            results.Add(await ToDtoAsync(user, cancellationToken));
        }

        return Result<IReadOnlyCollection<UserDto>>.Success(results);
    }

    public async Task<Result<UserDto>> UpdateAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure("User was not found.");
        }

        var rolesResult = ParseRoles(request.Roles);
        if (!rolesResult.IsSuccess)
        {
            return Result<UserDto>.Failure(rolesResult.Error!);
        }

        user.SetActive(request.IsActive, dateTimeProvider.UtcNow);
        await userRepository.ReplaceRolesAsync(user.Id, rolesResult.Value!, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UserDto>.Success(await ToDtoAsync(user, cancellationToken));
    }

    private async Task<UserDto> ToDtoAsync(Domain.Entities.User user, CancellationToken cancellationToken)
    {
        var roles = await userRepository.GetRolesAsync(user.Id, cancellationToken);
        return new UserDto(
            user.Id,
            user.Auth0UserId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            roles.Select(ToRoleName).ToArray());
    }

    private static Result<IReadOnlyCollection<UserRole>> ParseRoles(IReadOnlyCollection<string> roleNames)
    {
        if (roleNames.Count == 0)
        {
            return Result<IReadOnlyCollection<UserRole>>.Failure("At least one user role is required.");
        }

        var roles = new List<UserRole>();
        foreach (var roleName in roleNames)
        {
            var role = roleName.Trim() switch
            {
                "Visitor" => UserRole.Visitor,
                "RegisteredUser" or "Registered User" => UserRole.RegisteredUser,
                "Broker" => UserRole.Broker,
                "Administrator" => UserRole.Administrator,
                _ => (UserRole?)null
            };

            if (role is null)
            {
                return Result<IReadOnlyCollection<UserRole>>.Failure($"Unknown user role: {roleName}.");
            }

            roles.Add(role.Value);
        }

        return Result<IReadOnlyCollection<UserRole>>.Success(roles.Distinct().ToArray());
    }

    private static string ToRoleName(UserRole role) => role switch
    {
        UserRole.RegisteredUser => "Registered User",
        _ => role.ToString()
    };
}
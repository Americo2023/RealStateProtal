using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Users;

public sealed record UserDto(
    Guid Id,
    string Auth0UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    UserRole Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateUserRequest(
    string Auth0UserId,
    string Email,
    string FirstName,
    string LastName);
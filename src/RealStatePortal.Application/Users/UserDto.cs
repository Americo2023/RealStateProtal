namespace RealStatePortal.Application.Users;

public sealed record UserDto(
    Guid Id,
    string Auth0UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    IReadOnlyCollection<string> Roles);
namespace RealStatePortal.Application.Users;

public sealed record UpdateUserRequest(
    bool IsActive,
    IReadOnlyCollection<string> Roles);
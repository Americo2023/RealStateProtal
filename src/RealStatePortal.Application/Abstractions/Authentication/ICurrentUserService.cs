namespace RealStatePortal.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Auth0UserId { get; }
    bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Roles { get; }
}
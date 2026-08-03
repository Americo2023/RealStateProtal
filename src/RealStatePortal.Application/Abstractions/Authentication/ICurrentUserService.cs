using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsInRole(UserRole role);
}
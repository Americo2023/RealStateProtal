using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Domain.Entities;

public sealed class UserRoleAssignment : Entity
{
    private UserRoleAssignment()
        : base()
    {
    }

    public UserRoleAssignment(Guid userId, UserRole role, Guid? id = null)
        : base(id)
    {
        UserId = Guard.Required(userId, nameof(userId));
        Role = role;
    }

    public Guid UserId { get; }
    public UserRole Role { get; }
}
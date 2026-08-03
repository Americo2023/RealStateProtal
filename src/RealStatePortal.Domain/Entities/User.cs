using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Domain.Entities;

public sealed class User : Entity
{
    public User(Guid id, string auth0UserId, string email, string firstName, string lastName, DateTimeOffset createdAt)
        : base(id)
    {
        Auth0UserId = Required(auth0UserId, nameof(auth0UserId));
        Email = Required(email, nameof(email));
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        IsActive = true;
        Role = UserRole.RegisteredUser;
    }

    private User()
        : base(Guid.NewGuid())
    {
        Auth0UserId = string.Empty;
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public string Auth0UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void ChangeRole(UserRole role, DateTimeOffset updatedAt)
    {
        Role = role;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    private static string Required(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Value is required.", name)
        : value.Trim();
}

using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class User : Entity
{
    public User(
        string auth0UserId,
        string email,
        string firstName,
        string lastName,
        bool isActive = true,
        DateTimeOffset? createdAt = null,
        Guid? id = null)
        : base(id)
    {
        Auth0UserId = Guard.Required(auth0UserId, nameof(auth0UserId));
        Email = Guard.Required(email, nameof(email));
        FirstName = Guard.Required(firstName, nameof(firstName));
        LastName = Guard.Required(lastName, nameof(lastName));
        IsActive = isActive;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string Auth0UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void UpdateProfile(string email, string firstName, string lastName, DateTimeOffset occurredAt)
    {
        Email = Guard.Required(email, nameof(email));
        FirstName = Guard.Required(firstName, nameof(firstName));
        LastName = Guard.Required(lastName, nameof(lastName));
        UpdatedAt = occurredAt;
    }

    public void SetActive(bool isActive, DateTimeOffset occurredAt)
    {
        IsActive = isActive;
        UpdatedAt = occurredAt;
    }
}
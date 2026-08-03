using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class BrokerProfile : Entity
{
    public BrokerProfile(Guid id, Guid userId, string fullName, string email, string? phone, string? bio)
        : base(id)
    {
        UserId = userId == Guid.Empty ? throw new ArgumentException("User id is required.", nameof(userId)) : userId;
        FullName = Required(fullName, nameof(fullName));
        Email = Required(email, nameof(email));
        Phone = phone?.Trim();
        Bio = bio?.Trim();
        IsActive = true;
    }

    private BrokerProfile()
        : base(Guid.NewGuid())
    {
        FullName = string.Empty;
        Email = string.Empty;
    }

    public Guid UserId { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Bio { get; private set; }
    public bool IsActive { get; private set; }

    public void Deactivate() => IsActive = false;

    private static string Required(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Value is required.", name)
        : value.Trim();
}

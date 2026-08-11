using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class BrokerProfile : Entity
{
    private BrokerProfile()
        : base()
    {
        FullName = null!;
        Email = null!;
        Phone = null!;
        Bio = null!;
    }

    public BrokerProfile(
        Guid userId,
        string fullName,
        string email,
        string phone,
        string bio,
        bool isActive = true,
        Guid? id = null)
        : base(id)
    {
        UserId = Guard.Required(userId, nameof(userId));
        FullName = Guard.Required(fullName, nameof(fullName));
        Email = Guard.Required(email, nameof(email));
        Phone = Guard.Required(phone, nameof(phone));
        Bio = Guard.Required(bio, nameof(bio));
        IsActive = isActive;
    }

    public Guid UserId { get; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string Bio { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(string fullName, string email, string phone, string bio)
    {
        FullName = Guard.Required(fullName, nameof(fullName));
        Email = Guard.Required(email, nameof(email));
        Phone = Guard.Required(phone, nameof(phone));
        Bio = Guard.Required(bio, nameof(bio));
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
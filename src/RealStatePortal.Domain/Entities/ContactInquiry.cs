using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class ContactInquiry : Entity
{
    public ContactInquiry(Guid id, Guid propertyId, string visitorName, string visitorEmail, string? visitorPhone, string message, DateTimeOffset createdAt)
        : base(id)
    {
        PropertyId = propertyId == Guid.Empty ? throw new ArgumentException("Property id is required.", nameof(propertyId)) : propertyId;
        VisitorName = Required(visitorName, nameof(visitorName));
        VisitorEmail = Required(visitorEmail, nameof(visitorEmail));
        VisitorPhone = visitorPhone?.Trim();
        Message = Required(message, nameof(message));
        CreatedAt = createdAt;
    }

    private ContactInquiry()
        : base(Guid.NewGuid())
    {
        VisitorName = string.Empty;
        VisitorEmail = string.Empty;
        Message = string.Empty;
    }

    public Guid PropertyId { get; private set; }
    public string VisitorName { get; private set; }
    public string VisitorEmail { get; private set; }
    public string? VisitorPhone { get; private set; }
    public string Message { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private static string Required(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Value is required.", name)
        : value.Trim();
}

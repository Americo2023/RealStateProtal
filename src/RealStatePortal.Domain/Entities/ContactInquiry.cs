using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class ContactInquiry : Entity
{
    public ContactInquiry(
        Guid propertyId,
        string visitorName,
        string visitorEmail,
        string? visitorPhone,
        string message,
        DateTimeOffset? createdAt = null,
        Guid? id = null)
        : base(id)
    {
        PropertyId = Guard.Required(propertyId, nameof(propertyId));
        VisitorName = Guard.Required(visitorName, nameof(visitorName));
        VisitorEmail = Guard.Required(visitorEmail, nameof(visitorEmail));
        VisitorPhone = string.IsNullOrWhiteSpace(visitorPhone) ? null : visitorPhone.Trim();
        Message = Guard.Required(message, nameof(message));
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid PropertyId { get; }
    public string VisitorName { get; }
    public string VisitorEmail { get; }
    public string? VisitorPhone { get; }
    public string Message { get; }
    public DateTimeOffset CreatedAt { get; }
}
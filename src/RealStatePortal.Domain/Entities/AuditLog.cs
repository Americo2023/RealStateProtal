using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class AuditLog : Entity
{
    private AuditLog()
        : base()
    {
        EntityName = null!;
        Action = null!;
        Details = null!;
    }

    public AuditLog(
        string entityName,
        Guid entityId,
        string action,
        Guid? changedByUserId,
        DateTimeOffset changedAt,
        string details,
        Guid? id = null)
        : base(id)
    {
        EntityName = Guard.Required(entityName, nameof(entityName));
        EntityId = Guard.Required(entityId, nameof(entityId));
        Action = Guard.Required(action, nameof(action));
        ChangedByUserId = changedByUserId;
        ChangedAt = changedAt;
        Details = Guard.Required(details, nameof(details));
    }

    public string EntityName { get; }
    public Guid EntityId { get; }
    public string Action { get; }
    public Guid? ChangedByUserId { get; }
    public DateTimeOffset ChangedAt { get; }
    public string Details { get; }
}
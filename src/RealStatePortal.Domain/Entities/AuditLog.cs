using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class AuditLog : Entity
{
    public AuditLog(Guid id, string entityName, Guid entityId, string action, Guid? changedByUserId, DateTimeOffset changedAt, string? details)
        : base(id)
    {
        EntityName = Required(entityName, nameof(entityName));
        EntityId = entityId == Guid.Empty ? throw new ArgumentException("Entity id is required.", nameof(entityId)) : entityId;
        Action = Required(action, nameof(action));
        ChangedByUserId = changedByUserId;
        ChangedAt = changedAt;
        Details = details?.Trim();
    }

    private AuditLog()
        : base(Guid.NewGuid())
    {
        EntityName = string.Empty;
        Action = string.Empty;
    }

    public string EntityName { get; private set; }
    public Guid EntityId { get; private set; }
    public string Action { get; private set; }
    public Guid? ChangedByUserId { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public string? Details { get; private set; }

    private static string Required(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Value is required.", name)
        : value.Trim();
}

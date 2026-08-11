using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class Favorite : Entity
{
    private Favorite()
        : base()
    {
    }

    public Favorite(Guid userId, Guid propertyId, DateTimeOffset? createdAt = null, Guid? id = null)
        : base(id)
    {
        UserId = Guard.Required(userId, nameof(userId));
        PropertyId = Guard.Required(propertyId, nameof(propertyId));
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid UserId { get; }
    public Guid PropertyId { get; }
    public DateTimeOffset CreatedAt { get; }
}
using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Entities;

public sealed class Favorite : Entity
{
    public Favorite(Guid id, Guid userId, Guid propertyId, DateTimeOffset createdAt)
        : base(id)
    {
        UserId = RequiredId(userId, nameof(userId));
        PropertyId = RequiredId(propertyId, nameof(propertyId));
        CreatedAt = createdAt;
    }

    private Favorite()
        : base(Guid.NewGuid())
    {
    }

    public Guid UserId { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private static Guid RequiredId(Guid value, string name) => value == Guid.Empty
        ? throw new ArgumentException("Id is required.", name)
        : value;
}

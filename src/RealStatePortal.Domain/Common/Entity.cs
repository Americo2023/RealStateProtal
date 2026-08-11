namespace RealStatePortal.Domain.Common;

public abstract class Entity
{
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }

    public Guid Id { get; protected init; }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && GetType() == other.GetType() && Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
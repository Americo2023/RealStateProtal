namespace RealStatePortal.Domain.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid? id = null)
        : base(id)
    {
    }
}
using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Events;

public sealed record PropertySoldEvent(Guid PropertyId, DateTimeOffset OccurredAt) : IDomainEvent;
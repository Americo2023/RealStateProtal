using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Events;

public sealed record PropertyDeletedEvent(Guid PropertyId, DateTimeOffset OccurredAt) : IDomainEvent;
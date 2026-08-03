using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.Events;

public sealed record PropertyPublishedEvent(Guid PropertyId, DateTimeOffset OccurredAt) : IDomainEvent;
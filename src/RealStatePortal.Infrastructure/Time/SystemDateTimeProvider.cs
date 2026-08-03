using RealStatePortal.Application.Abstractions.Time;

namespace RealStatePortal.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
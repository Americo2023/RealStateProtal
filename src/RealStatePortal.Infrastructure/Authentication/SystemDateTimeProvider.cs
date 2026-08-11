using RealStatePortal.Application.Abstractions.Time;

namespace RealStatePortal.Infrastructure.Authentication;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
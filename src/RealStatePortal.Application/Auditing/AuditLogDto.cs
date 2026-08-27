namespace RealStatePortal.Application.Auditing;

public sealed record AuditLogDto(
    Guid Id,
    string EntityName,
    Guid EntityId,
    string Action,
    Guid? ChangedByUserId,
    DateTimeOffset ChangedAt,
    string Details);

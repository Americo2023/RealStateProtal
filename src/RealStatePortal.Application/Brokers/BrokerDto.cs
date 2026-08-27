namespace RealStatePortal.Application.Brokers;

public sealed record BrokerDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string Phone,
    string Bio,
    bool IsActive);
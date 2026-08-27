using System.ComponentModel.DataAnnotations;

namespace RealStatePortal.Application.Brokers;

public sealed record UpdateBrokerRequest(
    [param: Required, StringLength(200)] string FullName,
    [param: Required, EmailAddress, StringLength(320)] string Email,
    [param: Required, StringLength(40)] string Phone,
    [param: Required, StringLength(2000)] string Bio,
    bool IsActive);
using System.ComponentModel.DataAnnotations;

namespace RealStatePortal.Application.ContactInquiries;

public sealed record CreateContactInquiryRequest(
    Guid PropertyId,
    [param: Required, StringLength(150)]
    string VisitorName,
    [param: Required, EmailAddress, StringLength(320)]
    string VisitorEmail,
    [param: Phone, StringLength(40)]
    string? VisitorPhone,
    [param: Required, StringLength(4000)]
    string Message);
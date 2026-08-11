namespace RealStatePortal.Application.ContactInquiries;

public sealed record CreateContactInquiryRequest(
    Guid PropertyId,
    string VisitorName,
    string VisitorEmail,
    string? VisitorPhone,
    string Message);
namespace RealStatePortal.Application.ContactInquiries;

public sealed record ContactInquiryDto(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    string PropertyReferenceNumber,
    string VisitorName,
    string VisitorEmail,
    string? VisitorPhone,
    string Message,
    DateTimeOffset CreatedAt);
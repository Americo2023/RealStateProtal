namespace RealStatePortal.Application.Abstractions.Email;

public sealed record EmailMessage(
    string Recipient,
    string Subject,
    string Body,
    IReadOnlyCollection<string>? CarbonCopyRecipients = null);
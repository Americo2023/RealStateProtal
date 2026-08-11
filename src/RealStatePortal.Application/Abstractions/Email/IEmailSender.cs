namespace RealStatePortal.Application.Abstractions.Email;

public sealed record EmailMessage(string To, string Subject, string Body, string? Cc = null);

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
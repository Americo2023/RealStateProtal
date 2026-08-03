using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RealStatePortal.Application.Abstractions.Email;

namespace RealStatePortal.Infrastructure.Email;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        var host = RequiredConfiguration("Email:Smtp:Host");
        var port = configuration.GetValue<int?>("Email:Smtp:Port") ?? 587;
        var userName = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];
        var sender = RequiredConfiguration("Email:From");

        using var mail = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = false
        };
        mail.To.Add(new MailAddress(message.Recipient));
        foreach (var carbonCopyRecipient in message.CarbonCopyRecipients ?? [])
        {
            mail.CC.Add(new MailAddress(carbonCopyRecipient));
        }

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = configuration.GetValue("Email:Smtp:UseSsl", true),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
        if (!string.IsNullOrWhiteSpace(userName))
        {
            client.Credentials = new NetworkCredential(userName, password);
        }

        await client.SendMailAsync(mail, cancellationToken);
    }

    private string RequiredConfiguration(string key) => configuration[key]
        ?? throw new InvalidOperationException($"Configuration value '{key}' is required to send email.");
}
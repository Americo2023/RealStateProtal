using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RealStatePortal.Application.Abstractions.Email;

namespace RealStatePortal.Infrastructure.Email;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var host = RequiredConfiguration("Email:Smtp:Host");
        var from = RequiredConfiguration("Email:FromAddress");
        var port = int.TryParse(configuration["Email:Smtp:Port"], out var configuredPort) ? configuredPort : 587;
        var username = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];
        var enableSsl = !bool.TryParse(configuration["Email:Smtp:EnableSsl"], out var configuredEnableSsl) || configuredEnableSsl;

        using var mailMessage = new MailMessage(from, message.To)
        {
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = false
        };

        if (!string.IsNullOrWhiteSpace(message.Cc))
        {
            mailMessage.CC.Add(message.Cc);
        }

        using var smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = string.IsNullOrWhiteSpace(username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(username, password)
        };

#pragma warning disable SYSLIB0014
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
#pragma warning restore SYSLIB0014
    }

    private string RequiredConfiguration(string key) =>
        configuration[key] ?? throw new InvalidOperationException($"Missing required configuration: {key}.");
}
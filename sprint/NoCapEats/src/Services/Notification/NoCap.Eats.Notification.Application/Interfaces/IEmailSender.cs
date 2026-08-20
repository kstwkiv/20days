// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Notification.Application.Interfaces;

/// <summary>Abstraction over an email delivery provider (e.g. SendGrid, SMTP, console).</summary>
public interface IEmailSender
{
    /// <summary>Sends an HTML email to a single recipient.</summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Subject line of the email.</param>
    /// <param name="htmlBody">HTML body content of the email.</param>
    /// <param name="ct">Cancellation token.</param>
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}

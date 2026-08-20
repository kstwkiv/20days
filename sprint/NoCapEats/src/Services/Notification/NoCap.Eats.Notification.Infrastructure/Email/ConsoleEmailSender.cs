// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.Extensions.Logging;
using NoCap.Eats.Notification.Application.Interfaces;

namespace NoCap.Eats.Notification.Infrastructure.Email;

/// <summary>
/// Development implementation of <see cref="IEmailSender"/> that writes email content
/// to the application log instead of sending real messages.
/// Swap this registration for a real provider (e.g. SendGridEmailSender) before going to production.
/// </summary>
public class ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) : IEmailSender
{
    /// <inheritdoc/>
    /// <remarks>Writes a structured log entry at Information level and returns immediately.</remarks>
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL] To: {To} | Subject: {Subject}\n{Body}",
            to, subject, htmlBody);

        return Task.CompletedTask;
    }
}

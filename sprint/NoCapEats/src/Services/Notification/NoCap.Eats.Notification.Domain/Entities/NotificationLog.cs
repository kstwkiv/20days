// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Notification.Domain.Enums;

namespace NoCap.Eats.Notification.Domain.Entities;

/// <summary>
/// Audit record persisted for every notification attempted by the Notification service.
/// Captures the recipient, channel, message content, outcome, and any error details.
/// </summary>
public class NotificationLog
{
    /// <summary>Unique identifier of this log entry.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the user the notification was sent to, or <c>null</c> for system notifications.</summary>
    public Guid? UserId { get; private set; }

    /// <summary>Email address or phone number the notification was sent to.</summary>
    public string Recipient { get; private set; } = default!;

    /// <summary>Subject line of the notification (email subject or push title).</summary>
    public string Subject { get; private set; } = default!;

    /// <summary>Full body content of the notification.</summary>
    public string Body { get; private set; } = default!;

    /// <summary>Channel through which the notification was dispatched.</summary>
    public NotificationChannel Channel { get; private set; }

    /// <summary>Whether the notification was delivered successfully.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>Error message from the provider if delivery failed, otherwise <c>null</c>.</summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>UTC timestamp when the notification was attempted.</summary>
    public DateTime SentAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected NotificationLog() { }

    /// <summary>Creates a log entry for a successfully delivered notification.</summary>
    /// <param name="userId">ID of the recipient user, or <c>null</c>.</param>
    /// <param name="recipient">Email or phone of the recipient.</param>
    /// <param name="subject">Subject / title of the message.</param>
    /// <param name="body">Full message body.</param>
    /// <param name="channel">Channel used to deliver the notification.</param>
    /// <returns>A <see cref="NotificationLog"/> with <see cref="IsSuccess"/> set to <c>true</c>.</returns>
    public static NotificationLog Success(
        Guid? userId, string recipient, string subject,
        string body, NotificationChannel channel) => new()
    {
        UserId    = userId,
        Recipient = recipient,
        Subject   = subject,
        Body      = body,
        Channel   = channel,
        IsSuccess = true
    };

    /// <summary>Creates a log entry for a failed notification attempt.</summary>
    /// <param name="userId">ID of the intended recipient user, or <c>null</c>.</param>
    /// <param name="recipient">Email or phone of the intended recipient.</param>
    /// <param name="subject">Subject / title of the message.</param>
    /// <param name="body">Full message body.</param>
    /// <param name="channel">Channel that was attempted.</param>
    /// <param name="error">Error message from the sending provider.</param>
    /// <returns>A <see cref="NotificationLog"/> with <see cref="IsSuccess"/> set to <c>false</c>.</returns>
    public static NotificationLog Failure(
        Guid? userId, string recipient, string subject,
        string body, NotificationChannel channel, string error) => new()
    {
        UserId       = userId,
        Recipient    = recipient,
        Subject      = subject,
        Body         = body,
        Channel      = channel,
        IsSuccess    = false,
        ErrorMessage = error
    };
}

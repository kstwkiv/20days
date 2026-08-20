// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Notification.Domain.Enums;

/// <summary>
/// Identifies the delivery channel used to send a notification.
/// Used in <see cref="Entities.NotificationLog"/> to record how a message was dispatched.
/// </summary>
public enum NotificationChannel
{
    /// <summary>Notification sent via email (SMTP / SendGrid).</summary>
    Email = 0,

    /// <summary>Notification sent via SMS.</summary>
    Sms   = 1,

    /// <summary>Notification sent via mobile push notification.</summary>
    Push  = 2
}

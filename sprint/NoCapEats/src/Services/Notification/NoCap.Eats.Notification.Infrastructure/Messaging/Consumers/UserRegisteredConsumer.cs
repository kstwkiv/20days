// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.Extensions.Logging;
using NoCap.Eats.BuildingBlocks.Events;
using NoCap.Eats.Notification.Application.Interfaces;
using NoCap.Eats.Notification.Application.Templates;
using NoCap.Eats.Notification.Domain.Entities;
using NoCap.Eats.Notification.Domain.Enums;

namespace NoCap.Eats.Notification.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumes <see cref="UserRegisteredEvent"/> and sends a welcome email to the new user.
/// Logs the outcome (success or failure) as a <see cref="NotificationLog"/> record.
/// </summary>
public class UserRegisteredConsumer(
    IEmailSender                    emailSender,
    INotificationLogRepository      logRepo,
    ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegisteredEvent>
{
    /// <summary>Sends a welcome email and persists the notification audit record.</summary>
    /// <param name="context">MassTransit consume context wrapping the <see cref="UserRegisteredEvent"/>.</param>
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var ev = context.Message;
        var (subject, body) = EmailTemplates.WelcomeEmail(ev.FullName);

        NotificationLog log;
        try
        {
            await emailSender.SendAsync(ev.Email, subject, body, context.CancellationToken);
            log = NotificationLog.Success(ev.UserId, ev.Email, subject, body, NotificationChannel.Email);
            logger.LogInformation("Welcome email sent to {Email}", ev.Email);
        }
        catch (Exception ex)
        {
            // Never let a failed send crash the consumer — log and continue
            log = NotificationLog.Failure(ev.UserId, ev.Email, subject, body, NotificationChannel.Email, ex.Message);
            logger.LogWarning(ex, "Failed to send welcome email to {Email}", ev.Email);
        }

        await logRepo.AddAsync(log, context.CancellationToken);
        await logRepo.SaveChangesAsync(context.CancellationToken);
    }
}

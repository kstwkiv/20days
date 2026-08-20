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
/// Consumes <see cref="OrderStatusChangedEvent"/> and sends a status update email
/// for key transitions. Minor intermediate statuses are silently skipped.
/// </summary>
public class OrderStatusChangedConsumer(
    IEmailSender                        emailSender,
    INotificationLogRepository          logRepo,
    ILogger<OrderStatusChangedConsumer> logger) : IConsumer<OrderStatusChangedEvent>
{
    /// <summary>
    /// Statuses that are meaningful enough to warrant customer notification.
    /// Other status changes (e.g. Preparing) are intentionally filtered out.
    /// </summary>
    private static readonly HashSet<string> _notifyStatuses =
        ["Confirmed", "ReadyForPickup", "OutForDelivery", "Delivered", "Cancelled", "Rejected"];

    /// <summary>Sends a status update email if the new status is in the notify set.</summary>
    /// <param name="context">MassTransit consume context wrapping the <see cref="OrderStatusChangedEvent"/>.</param>
    public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
    {
        var ev = context.Message;

        // Skip status changes that customers don't need to be notified about
        if (!_notifyStatuses.Contains(ev.NewStatus))
            return;

        // NOTE: CustomerId is available but not the email address.
        // A production implementation would query Identity or a read model for the email.
        var recipientPlaceholder = $"customer-{ev.CustomerId}";
        var (subject, body) = EmailTemplates.OrderStatusChanged(
            recipientPlaceholder, ev.OrderId, ev.NewStatus);

        NotificationLog log;
        try
        {
            await emailSender.SendAsync(recipientPlaceholder, subject, body, context.CancellationToken);
            log = NotificationLog.Success(ev.CustomerId, recipientPlaceholder, subject, body, NotificationChannel.Email);
            logger.LogInformation(
                "Status change email sent for order {OrderId} → {Status}", ev.OrderId, ev.NewStatus);
        }
        catch (Exception ex)
        {
            log = NotificationLog.Failure(ev.CustomerId, recipientPlaceholder, subject, body, NotificationChannel.Email, ex.Message);
            logger.LogWarning(ex, "Failed to send status change email for order {OrderId}", ev.OrderId);
        }

        await logRepo.AddAsync(log, context.CancellationToken);
        await logRepo.SaveChangesAsync(context.CancellationToken);
    }
}

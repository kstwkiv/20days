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
/// Consumes <see cref="OrderPlacedEvent"/> and sends an order confirmation email to the customer.
/// Logs the outcome as a <see cref="NotificationLog"/> record.
/// </summary>
public class OrderPlacedConsumer(
    IEmailSender                 emailSender,
    INotificationLogRepository   logRepo,
    ILogger<OrderPlacedConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    /// <summary>Sends the order confirmation email and persists the audit record.</summary>
    /// <param name="context">MassTransit consume context wrapping the <see cref="OrderPlacedEvent"/>.</param>
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var ev = context.Message;

        // Build the email using the pre-formatted template
        var (subject, body) = EmailTemplates.OrderPlaced(
            ev.CustomerName, ev.OrderId, ev.TotalAmount);

        NotificationLog log;
        try
        {
            await emailSender.SendAsync(
                ev.CustomerName, subject, body, context.CancellationToken);
            log = NotificationLog.Success(ev.CustomerId, ev.CustomerName, subject, body, NotificationChannel.Email);
            logger.LogInformation("Order placed email sent for order {OrderId}", ev.OrderId);
        }
        catch (Exception ex)
        {
            log = NotificationLog.Failure(ev.CustomerId, ev.CustomerName, subject, body, NotificationChannel.Email, ex.Message);
            logger.LogWarning(ex, "Failed to send order placed email for order {OrderId}", ev.OrderId);
        }

        await logRepo.AddAsync(log, context.CancellationToken);
        await logRepo.SaveChangesAsync(context.CancellationToken);
    }
}

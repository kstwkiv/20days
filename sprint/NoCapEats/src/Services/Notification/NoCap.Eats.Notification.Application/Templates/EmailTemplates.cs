// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Notification.Application.Templates;

/// <summary>
/// Factory methods that produce pre-formatted HTML email subject and body pairs
/// for each notification scenario in the NoCap Eats platform.
/// </summary>
public static class EmailTemplates
{
    /// <summary>Generates a welcome email for a newly registered user.</summary>
    /// <param name="fullName">Full name of the new user.</param>
    /// <returns>A (Subject, HtmlBody) tuple ready to pass to <see cref="Interfaces.IEmailSender"/>.</returns>
    public static (string Subject, string Body) WelcomeEmail(string fullName) => (
        "Welcome to NoCap Eats!",
        $"""
        <h2>Hi {fullName},</h2>
        <p>Welcome to <strong>NoCap Eats</strong>! Your account is ready.</p>
        <p>Start exploring restaurants near you.</p>
        """);

    /// <summary>Generates an order confirmation email for the customer.</summary>
    /// <param name="customerName">Name of the customer.</param>
    /// <param name="orderId">ID of the placed order (first 8 chars shown).</param>
    /// <param name="total">Total monetary value of the order.</param>
    /// <returns>A (Subject, HtmlBody) tuple.</returns>
    public static (string Subject, string Body) OrderPlaced(string customerName, Guid orderId, decimal total) => (
        $"Order Confirmed — #{orderId.ToString()[..8].ToUpper()}",
        $"""
        <h2>Hi {customerName},</h2>
        <p>Your order <strong>#{orderId.ToString()[..8].ToUpper()}</strong> has been placed.</p>
        <p>Total: <strong>${total:F2}</strong></p>
        <p>We'll notify you when the restaurant confirms it.</p>
        """);

    /// <summary>Generates an order status update email for the customer.</summary>
    /// <param name="customerName">Name of the customer.</param>
    /// <param name="orderId">ID of the order whose status changed.</param>
    /// <param name="newStatus">The new status string to display.</param>
    /// <returns>A (Subject, HtmlBody) tuple.</returns>
    public static (string Subject, string Body) OrderStatusChanged(
        string customerName, Guid orderId, string newStatus) => (
        $"Order #{orderId.ToString()[..8].ToUpper()} — {newStatus}",
        $"""
        <h2>Hi {customerName},</h2>
        <p>Your order <strong>#{orderId.ToString()[..8].ToUpper()}</strong> status has changed to:</p>
        <h3>{newStatus}</h3>
        """);

    /// <summary>Generates a new order notification email for the restaurant.</summary>
    /// <param name="orderId">ID of the incoming order.</param>
    /// <param name="total">Total value of the order.</param>
    /// <param name="itemCount">Number of distinct line items.</param>
    /// <returns>A (Subject, HtmlBody) tuple.</returns>
    public static (string Subject, string Body) NewOrderForRestaurant(Guid orderId, decimal total, int itemCount) => (
        $"New Order #{orderId.ToString()[..8].ToUpper()} Received",
        $"""
        <h2>New order received!</h2>
        <p>Order <strong>#{orderId.ToString()[..8].ToUpper()}</strong></p>
        <p>{itemCount} item(s) — Total: <strong>${total:F2}</strong></p>
        <p>Please confirm or reject it promptly.</p>
        """);
}

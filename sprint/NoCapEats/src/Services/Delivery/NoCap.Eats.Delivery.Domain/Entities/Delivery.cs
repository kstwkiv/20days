// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Delivery.Domain.Enums;
using NoCap.Eats.Delivery.Domain.Exceptions;

namespace NoCap.Eats.Delivery.Domain.Entities;

/// <summary>
/// Aggregate root representing a single delivery job for an order.
/// Created automatically when an order is placed and progresses through a status machine
/// driven by the assigned delivery agent.
/// </summary>
public class Delivery
{
    /// <summary>Unique identifier of this delivery record.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the order this delivery is for.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>Identifier of the customer who placed the order.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Identifier of the restaurant the food is collected from.</summary>
    public Guid RestaurantId { get; private set; }

    /// <summary>Identifier of the agent who accepted this delivery, or <c>null</c> if unassigned.</summary>
    public Guid? AgentId { get; private set; }

    /// <summary>Full name of the customer for delivery confirmation.</summary>
    public string CustomerName { get; private set; } = default!;

    /// <summary>Contact phone number of the customer.</summary>
    public string CustomerPhone { get; private set; } = default!;

    /// <summary>Street address the food should be delivered to.</summary>
    public string DeliveryAddress { get; private set; } = default!;

    /// <summary>Total monetary value of the order (informational).</summary>
    public decimal OrderTotal { get; private set; }

    /// <summary>Current status of this delivery job.</summary>
    public DeliveryStatus Status { get; private set; } = DeliveryStatus.Pending;

    /// <summary>UTC timestamp when this delivery record was created.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the most recent status change.</summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the agent picked up the food, or <c>null</c> if not yet.</summary>
    public DateTime? PickedUpAt { get; private set; }

    /// <summary>UTC timestamp when the order was handed to the customer, or <c>null</c> if not yet.</summary>
    public DateTime? DeliveredAt { get; private set; }

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected Delivery() { }

    /// <summary>Creates a new pending delivery record from an order placement event.</summary>
    public Delivery(Guid orderId, Guid customerId, Guid restaurantId,
                    string customerName, string customerPhone,
                    string deliveryAddress, decimal orderTotal)
    {
        OrderId         = orderId;
        CustomerId      = customerId;
        RestaurantId    = restaurantId;
        CustomerName    = customerName;
        CustomerPhone   = customerPhone;
        DeliveryAddress = deliveryAddress;
        OrderTotal      = orderTotal;
    }

    /// <summary>
    /// Assigns an agent to this delivery and transitions it to Assigned.
    /// </summary>
    /// <param name="agentId">ID of the agent accepting the job.</param>
    /// <exception cref="DeliveryAlreadyAssignedException">Thrown when not in Pending state.</exception>
    public void AssignAgent(Guid agentId)
    {
        if (Status != DeliveryStatus.Pending)
            throw new DeliveryAlreadyAssignedException(OrderId);

        AgentId   = agentId;
        Status    = DeliveryStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Transitions from Assigned to PickedUp and records the pickup time.</summary>
    /// <param name="agentId">ID of the agent confirming pickup (must match <see cref="AgentId"/>).</param>
    /// <exception cref="UnauthorizedDeliveryAccessException">Thrown when agentId does not match.</exception>
    /// <exception cref="InvalidDeliveryStatusTransitionException">Thrown when not in Assigned state.</exception>
    public void MarkPickedUp(Guid agentId)
    {
        GuardAgent(agentId);
        GuardTransition(DeliveryStatus.Assigned, DeliveryStatus.PickedUp);
        Status     = DeliveryStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
        UpdatedAt  = DateTime.UtcNow;
    }

    /// <summary>Transitions from PickedUp to Delivered and records the delivery time.</summary>
    /// <param name="agentId">ID of the agent confirming delivery.</param>
    /// <exception cref="UnauthorizedDeliveryAccessException">Thrown when agentId does not match.</exception>
    /// <exception cref="InvalidDeliveryStatusTransitionException">Thrown when not in PickedUp state.</exception>
    public void MarkDelivered(Guid agentId)
    {
        GuardAgent(agentId);
        GuardTransition(DeliveryStatus.PickedUp, DeliveryStatus.Delivered);
        Status      = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt   = DateTime.UtcNow;
    }

    /// <summary>Marks the delivery as failed. Valid from Assigned or PickedUp states.</summary>
    /// <param name="agentId">ID of the agent reporting the failure.</param>
    /// <exception cref="UnauthorizedDeliveryAccessException">Thrown when agentId does not match.</exception>
    /// <exception cref="InvalidDeliveryStatusTransitionException">Thrown when in an invalid state.</exception>
    public void MarkFailed(Guid agentId)
    {
        GuardAgent(agentId);
        if (Status is not (DeliveryStatus.Assigned or DeliveryStatus.PickedUp))
            throw new InvalidDeliveryStatusTransitionException(Status.ToString(), "Failed");

        Status    = DeliveryStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Throws if the given agent ID does not match the assigned agent.</summary>
    /// <param name="agentId">The agent ID to verify.</param>
    /// <exception cref="UnauthorizedDeliveryAccessException">Thrown when IDs do not match.</exception>
    public void GuardAgent(Guid agentId)
    {
        if (AgentId != agentId)
            throw new UnauthorizedDeliveryAccessException(agentId, Id);
    }

    /// <summary>Validates that the current status matches the expected state.</summary>
    private void GuardTransition(DeliveryStatus expected, DeliveryStatus next)
    {
        if (Status != expected)
            throw new InvalidDeliveryStatusTransitionException(Status.ToString(), next.ToString());
    }
}

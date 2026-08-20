// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Order.Domain.Enums;
using NoCap.Eats.Order.Domain.Exceptions;

namespace NoCap.Eats.Order.Domain.Entities;

/// <summary>
/// Aggregate root representing a food order placed by a customer.
/// Owns the status machine that drives an order from Pending through to Delivered or Cancelled.
/// </summary>
public class Order
{
    /// <summary>Unique identifier of this order.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the customer who placed the order.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Identifier of the restaurant fulfilling the order.</summary>
    public Guid RestaurantId { get; private set; }

    /// <summary>Full name of the customer for delivery purposes.</summary>
    public string CustomerName { get; private set; } = default!;

    /// <summary>Contact phone number of the customer.</summary>
    public string CustomerPhone { get; private set; } = default!;

    /// <summary>Street address where the order should be delivered.</summary>
    public string DeliveryAddress { get; private set; } = default!;

    /// <summary>Optional delivery instructions from the customer.</summary>
    public string? DeliveryNotes { get; private set; }

    /// <summary>Current lifecycle status of the order.</summary>
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    /// <summary>Sum of all line item subtotals at time of placement.</summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>Identifier of the delivery agent assigned to this order, if any.</summary>
    public Guid? DeliveryAgentId { get; private set; }

    /// <summary>UTC timestamp when the order was placed.</summary>
    public DateTime PlacedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the most recent status change.</summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Backing field for the order's line items.</summary>
    private readonly List<OrderItem> _items = [];

    /// <summary>Read-only view of the order's line items.</summary>
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected Order() { }

    /// <summary>
    /// Creates a new order with the supplied customer details and line items.
    /// Calculates the total amount from the provided lines.
    /// </summary>
    /// <param name="customerId">ID of the ordering customer.</param>
    /// <param name="restaurantId">ID of the restaurant fulfilling the order.</param>
    /// <param name="customerName">Full name of the customer.</param>
    /// <param name="customerPhone">Contact phone of the customer.</param>
    /// <param name="deliveryAddress">Delivery destination address.</param>
    /// <param name="deliveryNotes">Optional special delivery instructions.</param>
    /// <param name="lines">Line items as (MenuItemId, Name, Quantity, UnitPrice) tuples.</param>
    /// <exception cref="EmptyOrderException">Thrown when no line items are provided.</exception>
    public Order(Guid customerId, Guid restaurantId, string customerName,
                 string customerPhone, string deliveryAddress, string? deliveryNotes,
                 IEnumerable<(Guid MenuItemId, string Name, int Qty, decimal Price)> lines)
    {
        if (!lines.Any())
            throw new EmptyOrderException();

        CustomerId      = customerId;
        RestaurantId    = restaurantId;
        CustomerName    = customerName;
        CustomerPhone   = customerPhone;
        DeliveryAddress = deliveryAddress;
        DeliveryNotes   = deliveryNotes;
        PlacedAt        = DateTime.UtcNow;

        foreach (var (menuItemId, name, qty, price) in lines)
            _items.Add(new OrderItem(Id, menuItemId, name, qty, price));

        // Total is locked at placement time; price changes don't affect existing orders
        TotalAmount = _items.Sum(i => i.Subtotal);
    }

    // ── Status transitions ────────────────────────────────────────────────────

    /// <summary>Transitions the order from Pending to Confirmed (restaurant accepted).</summary>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in Pending state.</exception>
    public void Confirm()
    {
        GuardTransition(OrderStatus.Pending, OrderStatus.Confirmed);
        Status    = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Transitions the order from Confirmed to Preparing (kitchen started).</summary>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in Confirmed state.</exception>
    public void StartPreparing()
    {
        GuardTransition(OrderStatus.Confirmed, OrderStatus.Preparing);
        Status    = OrderStatus.Preparing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Transitions from Preparing to ReadyForPickup (food is ready for agent).</summary>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in Preparing state.</exception>
    public void MarkReadyForPickup()
    {
        GuardTransition(OrderStatus.Preparing, OrderStatus.ReadyForPickup);
        Status    = OrderStatus.ReadyForPickup;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns a delivery agent and transitions the order to OutForDelivery.
    /// </summary>
    /// <param name="agentId">ID of the delivery agent picking up the order.</param>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in ReadyForPickup state.</exception>
    public void AssignDeliveryAgent(Guid agentId)
    {
        if (Status != OrderStatus.ReadyForPickup)
            throw new InvalidOrderStatusTransitionException(Status.ToString(), "OutForDelivery");

        DeliveryAgentId = agentId;
        Status          = OrderStatus.OutForDelivery;
        UpdatedAt       = DateTime.UtcNow;
    }

    /// <summary>Transitions the order from OutForDelivery to Delivered.</summary>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in OutForDelivery state.</exception>
    public void MarkDelivered()
    {
        GuardTransition(OrderStatus.OutForDelivery, OrderStatus.Delivered);
        Status    = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Cancels the order. Only valid when Pending or Confirmed.</summary>
    /// <exception cref="OrderNotCancellableException">Thrown when the order is past a cancellable state.</exception>
    public void Cancel()
    {
        var cancellable = new[] { OrderStatus.Pending, OrderStatus.Confirmed };
        if (!cancellable.Contains(Status))
            throw new OrderNotCancellableException(Id);

        Status    = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Rejects the order (restaurant declines). Only valid when Pending.</summary>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when not in Pending state.</exception>
    public void Reject()
    {
        GuardTransition(OrderStatus.Pending, OrderStatus.Rejected);
        Status    = OrderStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Throws if the requesting user is not the customer who placed this order.</summary>
    /// <param name="requestingCustomerId">The ID of the user attempting the action.</param>
    /// <exception cref="UnauthorizedOrderAccessException">Thrown when IDs do not match.</exception>
    public void GuardCustomer(Guid requestingCustomerId)
    {
        if (CustomerId != requestingCustomerId)
            throw new UnauthorizedOrderAccessException(requestingCustomerId, Id);
    }

    /// <summary>Ensures the current status matches the expected state before a transition.</summary>
    /// <param name="expected">The status required before transitioning.</param>
    /// <param name="next">The target status (used in the exception message).</param>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown when current status differs from expected.</exception>
    private void GuardTransition(OrderStatus expected, OrderStatus next)
    {
        if (Status != expected)
            throw new InvalidOrderStatusTransitionException(Status.ToString(), next.ToString());
    }
}

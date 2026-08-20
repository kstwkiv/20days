// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Order.Domain.Entities;

/// <summary>
/// A single line item within an <see cref="Order"/>.
/// Stores the snapshot of the menu item name and unit price at the time of ordering,
/// so subsequent catalog changes do not retroactively alter order history.
/// </summary>
public class OrderItem
{
    /// <summary>Unique identifier of this line item.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the parent order.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>Identifier of the menu item that was ordered.</summary>
    public Guid MenuItemId { get; private set; }

    /// <summary>Snapshot of the menu item's display name at order time.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Number of units ordered.</summary>
    public int Quantity { get; private set; }

    /// <summary>Price per unit at the time the order was placed.</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Computed line total: <see cref="Quantity"/> × <see cref="UnitPrice"/>.</summary>
    public decimal Subtotal => Quantity * UnitPrice;

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected OrderItem() { }

    /// <summary>Creates a new order line item.</summary>
    /// <param name="orderId">ID of the owning order.</param>
    /// <param name="menuItemId">ID of the menu item being ordered.</param>
    /// <param name="name">Snapshot of the menu item name.</param>
    /// <param name="quantity">Number of units.</param>
    /// <param name="unitPrice">Price per unit at order time.</param>
    public OrderItem(Guid orderId, Guid menuItemId, string name, int quantity, decimal unitPrice)
    {
        OrderId    = orderId;
        MenuItemId = menuItemId;
        Name       = name;
        Quantity   = quantity;
        UnitPrice  = unitPrice;
    }
}

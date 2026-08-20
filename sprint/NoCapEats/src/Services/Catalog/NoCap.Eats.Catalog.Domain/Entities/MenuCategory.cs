// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Domain.Entities;

/// <summary>
/// Represents a named grouping of menu items within a restaurant (e.g. "Starters", "Mains").
/// Contains an ordered list of <see cref="MenuItem"/> records.
/// </summary>
public class MenuCategory
{
    /// <summary>Unique identifier of this category.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the restaurant this category belongs to.</summary>
    public Guid RestaurantId { get; private set; }

    /// <summary>Display name of the category shown to customers.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Optional description explaining what this category contains.</summary>
    public string? Description { get; private set; }

    /// <summary>Display order position; lower values appear first.</summary>
    public int SortOrder { get; private set; }

    /// <summary>Whether this category is visible to customers.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>UTC timestamp when this category was created.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Backing field for menu items.</summary>
    private readonly List<MenuItem> _items = [];

    /// <summary>Read-only view of the items in this category.</summary>
    public IReadOnlyList<MenuItem> Items => _items.AsReadOnly();

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected MenuCategory() { }

    /// <summary>Creates a new menu category under the specified restaurant.</summary>
    /// <param name="restaurantId">ID of the owning restaurant.</param>
    /// <param name="name">Display name of the category.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="sortOrder">Display order position.</param>
    public MenuCategory(Guid restaurantId, string name, string? description = null, int sortOrder = 0)
    {
        RestaurantId = restaurantId;
        Name         = name;
        Description  = description;
        SortOrder    = sortOrder;
    }

    /// <summary>Updates the category's display properties.</summary>
    public void Update(string name, string? description, int sortOrder)
    {
        Name        = name;
        Description = description;
        SortOrder   = sortOrder;
    }

    /// <summary>Hides this category from customers without deleting it.</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Makes this category visible to customers again.</summary>
    public void Reactivate() => IsActive = true;

    /// <summary>Adds a new menu item to this category and returns it.</summary>
    /// <param name="name">Display name of the item.</param>
    /// <param name="description">Description of the item.</param>
    /// <param name="price">Unit price.</param>
    /// <param name="imageUrl">Optional image URL.</param>
    /// <param name="isAvailable">Whether the item is currently orderable.</param>
    /// <returns>The newly created <see cref="MenuItem"/>.</returns>
    public MenuItem AddItem(string name, string description, decimal price,
                            string? imageUrl = null, bool isAvailable = true)
    {
        var item = new MenuItem(Id, name, description, price, imageUrl, isAvailable);
        _items.Add(item);
        return item;
    }
}

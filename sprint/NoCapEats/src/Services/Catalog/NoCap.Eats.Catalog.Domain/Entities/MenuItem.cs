// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Domain.Entities;

/// <summary>
/// Represents a single orderable item on a restaurant's menu.
/// Belongs to a <see cref="MenuCategory"/> and tracks price, availability, and an optional image.
/// </summary>
public class MenuItem
{
    /// <summary>Unique identifier of this menu item.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the parent menu category.</summary>
    public Guid CategoryId { get; private set; }

    /// <summary>Display name of the item shown to customers.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Description of the item including ingredients or preparation notes.</summary>
    public string Description { get; private set; } = default!;

    /// <summary>Current selling price of the item.</summary>
    public decimal Price { get; private set; }

    /// <summary>URL of the item's photo, or <c>null</c> if no image has been uploaded.</summary>
    public string? ImageUrl { get; private set; }

    /// <summary>Whether this item can currently be ordered by customers.</summary>
    public bool IsAvailable { get; private set; } = true;

    /// <summary>UTC timestamp when this item was first added to the menu.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the most recent update to this item.</summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected MenuItem() { }

    /// <summary>Creates a new menu item in the specified category.</summary>
    public MenuItem(Guid categoryId, string name, string description,
                    decimal price, string? imageUrl = null, bool isAvailable = true)
    {
        CategoryId  = categoryId;
        Name        = name;
        Description = description;
        Price       = price;
        ImageUrl    = imageUrl;
        IsAvailable = isAvailable;
    }

    /// <summary>Updates the item's name, description, price, and image URL.</summary>
    public void Update(string name, string description, decimal price, string? imageUrl)
    {
        Name        = name;
        Description = description;
        Price       = price;
        ImageUrl    = imageUrl;
        UpdatedAt   = DateTime.UtcNow;
    }

    /// <summary>Replaces the item's image URL.</summary>
    /// <param name="imageUrl">Public URL of the new image.</param>
    public void SetImage(string imageUrl) { ImageUrl = imageUrl; UpdatedAt = DateTime.UtcNow; }

    /// <summary>Marks the item as temporarily unavailable for ordering.</summary>
    public void MarkUnavailable() { IsAvailable = false; UpdatedAt = DateTime.UtcNow; }

    /// <summary>Marks the item as available for ordering again.</summary>
    public void MarkAvailable()   { IsAvailable = true;  UpdatedAt = DateTime.UtcNow; }
}

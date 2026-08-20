// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Application.DTOs;

/// <summary>Read-only projection of a single menu item returned to API callers.</summary>
/// <param name="Id">Unique identifier of the menu item.</param>
/// <param name="CategoryId">Identifier of the parent menu category.</param>
/// <param name="Name">Display name of the item.</param>
/// <param name="Description">Description including ingredients or preparation notes.</param>
/// <param name="Price">Current selling price.</param>
/// <param name="ImageUrl">Photo URL, or <c>null</c> if no image has been uploaded.</param>
/// <param name="IsAvailable">Whether this item can currently be ordered.</param>
public record MenuItemDto(
    Guid    Id,
    Guid    CategoryId,
    string  Name,
    string  Description,
    decimal Price,
    string? ImageUrl,
    bool    IsAvailable);

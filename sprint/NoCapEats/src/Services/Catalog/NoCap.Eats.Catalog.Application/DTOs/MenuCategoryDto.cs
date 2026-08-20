// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Application.DTOs;

/// <summary>Read-only projection of a menu category including its items.</summary>
/// <param name="Id">Unique identifier of the category.</param>
/// <param name="RestaurantId">Identifier of the owning restaurant.</param>
/// <param name="Name">Display name of the category.</param>
/// <param name="Description">Optional description.</param>
/// <param name="SortOrder">Display order position; lower values appear first.</param>
/// <param name="IsActive">Whether the category is currently visible to customers.</param>
/// <param name="Items">Menu items belonging to this category.</param>
public record MenuCategoryDto(
    Guid    Id,
    Guid    RestaurantId,
    string  Name,
    string? Description,
    int     SortOrder,
    bool    IsActive,
    IReadOnlyList<MenuItemDto> Items);

// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Application.Mappings;

/// <summary>
/// Static extension methods that map Catalog domain entities to their corresponding DTOs.
/// Uses direct positional mapping instead of AutoMapper to avoid reflection overhead.
/// </summary>
public static class MappingExtensions
{
    /// <summary>Maps a <see cref="Restaurant"/> to a lightweight <see cref="RestaurantDto"/>.</summary>
    public static RestaurantDto ToDto(this Restaurant r) => new(
        r.Id, r.OwnerId, r.Name, r.Description, r.Address,
        r.City, r.Phone, r.ImageUrl, r.CuisineType, r.Status, r.IsOpen, r.CreatedAt);

    /// <summary>Maps a <see cref="Restaurant"/> to a full <see cref="RestaurantDetailDto"/> including categories and items.</summary>
    public static RestaurantDetailDto ToDetailDto(this Restaurant r) => new(
        r.Id, r.OwnerId, r.Name, r.Description, r.Address,
        r.City, r.Phone, r.ImageUrl, r.CuisineType, r.Status, r.IsOpen, r.CreatedAt,
        r.Categories.Select(c => c.ToDto()).ToList());

    /// <summary>Maps a <see cref="MenuCategory"/> to a <see cref="MenuCategoryDto"/> including its items.</summary>
    public static MenuCategoryDto ToDto(this MenuCategory c) => new(
        c.Id, c.RestaurantId, c.Name, c.Description,
        c.SortOrder, c.IsActive,
        c.Items.Select(i => i.ToDto()).ToList());

    /// <summary>Maps a <see cref="MenuItem"/> to a <see cref="MenuItemDto"/>.</summary>
    public static MenuItemDto ToDto(this MenuItem i) => new(
        i.Id, i.CategoryId, i.Name, i.Description, i.Price, i.ImageUrl, i.IsAvailable);
}

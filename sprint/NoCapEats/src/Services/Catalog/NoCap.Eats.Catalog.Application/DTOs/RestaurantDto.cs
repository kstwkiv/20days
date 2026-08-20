// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Domain.Enums;

namespace NoCap.Eats.Catalog.Application.DTOs;

/// <summary>Lightweight restaurant summary returned in list responses.</summary>
/// <param name="Id">Unique identifier of the restaurant.</param>
/// <param name="OwnerId">Identifier of the owning RestaurantOwner user.</param>
/// <param name="Name">Display name of the restaurant.</param>
/// <param name="Description">Marketing description.</param>
/// <param name="Address">Street address.</param>
/// <param name="City">City for filtering.</param>
/// <param name="Phone">Contact phone number.</param>
/// <param name="ImageUrl">Cover image URL, or <c>null</c>.</param>
/// <param name="CuisineType">Optional cuisine label.</param>
/// <param name="Status">Current approval and operational status.</param>
/// <param name="IsOpen">Whether the restaurant is currently accepting orders.</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
public record RestaurantDto(
    Guid             Id,
    Guid             OwnerId,
    string           Name,
    string           Description,
    string           Address,
    string           City,
    string           Phone,
    string?          ImageUrl,
    string?          CuisineType,
    RestaurantStatus Status,
    bool             IsOpen,
    DateTime         CreatedAt);

/// <summary>Full restaurant detail including all menu categories and their items.</summary>
/// <param name="Categories">Ordered list of the restaurant's menu categories with items.</param>
public record RestaurantDetailDto(
    Guid             Id,
    Guid             OwnerId,
    string           Name,
    string           Description,
    string           Address,
    string           City,
    string           Phone,
    string?          ImageUrl,
    string?          CuisineType,
    RestaurantStatus Status,
    bool             IsOpen,
    DateTime         CreatedAt,
    IReadOnlyList<MenuCategoryDto> Categories);

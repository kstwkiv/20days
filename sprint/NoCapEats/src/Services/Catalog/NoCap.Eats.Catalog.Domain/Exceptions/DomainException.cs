// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Domain.Exceptions;

/// <summary>Base exception for all domain-level errors in the Catalog service.</summary>
/// <param name="message">Human-readable description of the error.</param>
public class DomainException(string message) : Exception(message);

/// <summary>Thrown when a restaurant with the given ID cannot be found.</summary>
/// <param name="id">The restaurant ID that was searched for.</param>
public class RestaurantNotFoundException(Guid id)
    : DomainException($"Restaurant '{id}' was not found.");

/// <summary>Thrown when a menu category with the given ID cannot be found.</summary>
/// <param name="id">The category ID that was searched for.</param>
public class MenuCategoryNotFoundException(Guid id)
    : DomainException($"Menu category '{id}' was not found.");

/// <summary>Thrown when a menu item with the given ID cannot be found.</summary>
/// <param name="id">The menu item ID that was searched for.</param>
public class MenuItemNotFoundException(Guid id)
    : DomainException($"Menu item '{id}' was not found.");

/// <summary>Thrown when a user attempts to modify a restaurant they do not own.</summary>
/// <param name="ownerId">The ID of the user making the request.</param>
/// <param name="restaurantId">The ID of the restaurant being accessed.</param>
public class UnauthorizedRestaurantAccessException(Guid ownerId, Guid restaurantId)
    : DomainException($"Owner '{ownerId}' does not own restaurant '{restaurantId}'.");

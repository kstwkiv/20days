// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.BuildingBlocks.Events;

/// <summary>
/// Published when a new user successfully registers.
/// Consumed by downstream services (e.g. Notifications, Catalog).
/// </summary>
/// <param name="UserId">Unique identifier of the newly registered user.</param>
/// <param name="FullName">Full display name of the user.</param>
/// <param name="Email">Email address of the user.</param>
/// <param name="Role">Role assigned to the user on registration.</param>
/// <param name="RegisteredAt">UTC timestamp when the registration occurred.</param>
public record UserRegisteredEvent(
    Guid   UserId,
    string FullName,
    string Email,
    string Role,
    DateTime RegisteredAt);

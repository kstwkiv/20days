namespace NoCap.Eats.BuildingBlocks.Events;

/// <summary>
/// Published when a new user successfully registers.
/// Consumed by downstream services (e.g. Notifications, Catalog).
/// </summary>
public record UserRegisteredEvent(
    Guid   UserId,
    string FullName,
    string Email,
    string Role,
    DateTime RegisteredAt);

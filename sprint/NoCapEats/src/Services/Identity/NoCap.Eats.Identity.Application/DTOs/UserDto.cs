// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Application.DTOs;

/// <summary>Read-only projection of a user account returned to API callers.</summary>
/// <param name="Id">Unique identifier of the user.</param>
/// <param name="FullName">Full display name of the user.</param>
/// <param name="Email">Email address and login identifier.</param>
/// <param name="MobileNumber">Mobile phone number.</param>
/// <param name="Role">Single role assigned to the user.</param>
/// <param name="IsActive">Whether the account is currently active.</param>
/// <param name="CreatedAt">UTC timestamp when the account was created.</param>
/// <param name="LastLoginAt">UTC timestamp of the most recent login, or <c>null</c> if never logged in.</param>
public record UserDto(
    Guid   Id,
    string FullName,
    string Email,
    string MobileNumber,
    string Role,
    bool   IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

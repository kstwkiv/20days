// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Application.Interfaces;

/// <summary>Data access contract for <see cref="AppUser"/> read operations.</summary>
public interface IUserRepository
{
    /// <summary>Returns the user with the given ID, or <c>null</c> if not found.</summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the user with the given email address, or <c>null</c> if not found.</summary>
    /// <param name="email">Email address to search for (case-insensitive).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if the email address is already registered.</summary>
    /// <param name="email">Email address to check.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}

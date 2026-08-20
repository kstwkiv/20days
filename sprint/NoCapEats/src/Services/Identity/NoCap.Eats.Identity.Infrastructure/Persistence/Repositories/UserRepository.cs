// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// Delegates reads to the <see cref="UserManager{TUser}"/> queryable,
/// ensuring normalized email comparisons are consistent with Identity's internal logic.
/// </summary>
public class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{
    /// <inheritdoc/>
    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Compares against <c>NormalizedEmail</c> for case-insensitive lookup.</remarks>
    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await userManager.Users.FirstOrDefaultAsync(
               u => u.NormalizedEmail == email.ToUpperInvariant(), ct);

    /// <inheritdoc/>
    /// <remarks>Uses <c>AnyAsync</c> to avoid loading the full user record for existence checks.</remarks>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(
               u => u.NormalizedEmail == email.ToUpperInvariant(), ct);
}

// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NoCap.Eats.Identity.Domain.Enums;

namespace NoCap.Eats.Identity.Infrastructure.Persistence;

/// <summary>
/// Seeds the required ASP.NET Core Identity roles into the database on startup.
/// Ensures all <see cref="UserRole"/> constants exist as <see cref="IdentityRole{Guid}"/> records.
/// Safe to call multiple times — skips roles that already exist (idempotent).
/// </summary>
public class IdentityDbSeeder(
    RoleManager<IdentityRole<Guid>> roleManager,
    ILogger<IdentityDbSeeder>       logger)
{
    /// <summary>
    /// Creates any missing roles from <see cref="UserRole.All"/>.
    /// Logs success or failure for each role individually.
    /// </summary>
    public async Task SeedRolesAsync()
    {
        foreach (var role in UserRole.All)
        {
            // Skip roles that are already present in the database
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));

            if (result.Succeeded)
                logger.LogInformation("Created role: {Role}", role);
            else
                logger.LogWarning("Failed to create role {Role}: {Errors}", role,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

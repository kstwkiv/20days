using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NoCap.Eats.Identity.Domain.Enums;

namespace NoCap.Eats.Identity.Infrastructure.Persistence;

/// <summary>
/// Ensures all <see cref="UserRole"/> constants exist as Identity roles.
/// Call once at startup — idempotent.
/// </summary>
public class IdentityDbSeeder(
    RoleManager<IdentityRole<Guid>> roleManager,
    ILogger<IdentityDbSeeder>       logger)
{
    public async Task SeedRolesAsync()
    {
        foreach (var role in UserRole.All)
        {
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

// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Identity service.
/// Extends <see cref="IdentityDbContext{TUser,TRole,TKey}"/> to include
/// the custom <see cref="AppUser"/> and persisted <see cref="RefreshToken"/> records.
/// All tables are placed under the "identity" schema.
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>Table containing all persisted BCrypt-hashed refresh tokens.</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply base Identity table mappings first
        base.OnModelCreating(builder);

        // Move all tables into the "identity" schema for clean separation
        builder.HasDefaultSchema("identity");

        // Configure AppUser table with custom column constraints
        builder.Entity<AppUser>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.MobileNumber).HasMaxLength(20).IsRequired();
        });

        // Rename default ASP.NET Core Identity tables to cleaner names
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        // Configure RefreshToken table with appropriate indexes for fast lookup
        builder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(r => r.Id);
            e.Property(r => r.TokenHash).HasMaxLength(256).IsRequired();
            // Index for fetching all tokens by user
            e.HasIndex(r => r.UserId);
            // Composite index for active token queries (user + revocation status)
            e.HasIndex(r => new { r.UserId, r.IsRevoked });
        });
    }
}

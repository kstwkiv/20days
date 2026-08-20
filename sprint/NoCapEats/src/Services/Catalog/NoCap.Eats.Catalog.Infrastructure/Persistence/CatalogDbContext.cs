// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Catalog service.
/// All tables are placed under the "catalog" schema.
/// Restaurants cascade-delete categories, and categories cascade-delete items.
/// </summary>
public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    /// <summary>Table containing all restaurant listings.</summary>
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();

    /// <summary>Table containing all menu categories.</summary>
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();

    /// <summary>Table containing all menu items.</summary>
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("catalog");

        builder.Entity<Restaurant>(e =>
        {
            e.ToTable("Restaurants");
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).HasMaxLength(150).IsRequired();
            e.Property(r => r.Description).HasMaxLength(1000).IsRequired();
            e.Property(r => r.Address).HasMaxLength(300).IsRequired();
            e.Property(r => r.City).HasMaxLength(100).IsRequired();
            e.Property(r => r.Phone).HasMaxLength(30).IsRequired();
            e.Property(r => r.CuisineType).HasMaxLength(100);
            e.Property(r => r.ImageUrl).HasMaxLength(500);
            // Store status as string for readability
            e.Property(r => r.Status).HasConversion<string>();
            e.HasIndex(r => r.OwnerId);
            // Composite index supports the common "browse by city, active only" query
            e.HasIndex(r => new { r.City, r.Status });

            e.HasMany(r => r.Categories)
             .WithOne()
             .HasForeignKey(c => c.RestaurantId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MenuCategory>(e =>
        {
            e.ToTable("MenuCategories");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.Property(c => c.Description).HasMaxLength(500);
            e.HasIndex(c => c.RestaurantId);

            e.HasMany(c => c.Items)
             .WithOne()
             .HasForeignKey(i => i.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MenuItem>(e =>
        {
            e.ToTable("MenuItems");
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).HasMaxLength(150).IsRequired();
            e.Property(i => i.Description).HasMaxLength(500).IsRequired();
            e.Property(i => i.Price).HasColumnType("decimal(18,2)");
            e.Property(i => i.ImageUrl).HasMaxLength(500);
            e.HasIndex(i => i.CategoryId);
        });
    }
}

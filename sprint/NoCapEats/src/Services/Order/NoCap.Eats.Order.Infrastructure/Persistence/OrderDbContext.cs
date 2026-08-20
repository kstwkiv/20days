// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Order.Domain.Entities;

namespace NoCap.Eats.Order.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Order service.
/// All tables are placed under the "orders" schema.
/// Orders cascade-delete their line items when removed.
/// </summary>
public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    /// <summary>Table containing all order aggregates.</summary>
    public DbSet<Domain.Entities.Order> Orders => Set<Domain.Entities.Order>();

    /// <summary>Table containing all order line items.</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Place all Order service tables under the "orders" schema
        builder.HasDefaultSchema("orders");

        builder.Entity<Domain.Entities.Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(o => o.Id);
            e.Property(o => o.CustomerName).HasMaxLength(150).IsRequired();
            e.Property(o => o.CustomerPhone).HasMaxLength(30).IsRequired();
            e.Property(o => o.DeliveryAddress).HasMaxLength(300).IsRequired();
            e.Property(o => o.DeliveryNotes).HasMaxLength(500);
            // Store status as string for readability in the database
            e.Property(o => o.Status).HasConversion<string>();
            e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            // Indexes for common query patterns
            e.HasIndex(o => o.CustomerId);
            e.HasIndex(o => o.RestaurantId);
            e.HasIndex(o => new { o.RestaurantId, o.Status });

            e.HasMany(o => o.Items)
             .WithOne()
             .HasForeignKey(i => i.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(e =>
        {
            e.ToTable("OrderItems");
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).HasMaxLength(150).IsRequired();
            e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            e.HasIndex(i => i.OrderId);
        });
    }
}

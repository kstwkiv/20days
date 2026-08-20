// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using DeliveryEntity = NoCap.Eats.Delivery.Domain.Entities.Delivery;

namespace NoCap.Eats.Delivery.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Delivery service.
/// All tables are placed under the "delivery" schema.
/// A unique index on OrderId enforces one delivery per order.
/// </summary>
public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
{
    /// <summary>Table containing all delivery job records.</summary>
    public DbSet<DeliveryEntity> Deliveries => Set<DeliveryEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("delivery");

        builder.Entity<DeliveryEntity>(e =>
        {
            e.ToTable("Deliveries");
            e.HasKey(d => d.Id);
            e.Property(d => d.CustomerName).HasMaxLength(150).IsRequired();
            e.Property(d => d.CustomerPhone).HasMaxLength(30).IsRequired();
            e.Property(d => d.DeliveryAddress).HasMaxLength(300).IsRequired();
            e.Property(d => d.OrderTotal).HasColumnType("decimal(18,2)");
            // Store status as string for readability in the database
            e.Property(d => d.Status).HasConversion<string>();
            // Unique index ensures only one delivery exists per order (idempotency)
            e.HasIndex(d => d.OrderId).IsUnique();
            e.HasIndex(d => d.AgentId);
            e.HasIndex(d => d.Status);
        });
    }
}

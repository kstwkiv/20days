// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Notification.Domain.Entities;

namespace NoCap.Eats.Notification.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Notification service.
/// All tables are placed under the "notifications" schema.
/// Only stores <see cref="NotificationLog"/> audit records — no aggregate state.
/// </summary>
public class NotificationDbContext(DbContextOptions<NotificationDbContext> options)
    : DbContext(options)
{
    /// <summary>Table containing an audit record for every notification attempt.</summary>
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("notifications");

        builder.Entity<NotificationLog>(e =>
        {
            e.ToTable("NotificationLogs");
            e.HasKey(n => n.Id);
            e.Property(n => n.Recipient).HasMaxLength(200).IsRequired();
            e.Property(n => n.Subject).HasMaxLength(300).IsRequired();
            e.Property(n => n.Body).HasMaxLength(4000).IsRequired();
            // Store channel as string for readability
            e.Property(n => n.Channel).HasConversion<string>();
            e.Property(n => n.ErrorMessage).HasMaxLength(1000);
            // Indexes for querying a user's notification history and time-range reports
            e.HasIndex(n => n.UserId);
            e.HasIndex(n => n.SentAt);
        });
    }
}

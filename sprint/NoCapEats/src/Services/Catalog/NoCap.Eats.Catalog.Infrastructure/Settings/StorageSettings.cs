// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration options for Azure Blob Storage image uploads.
/// Bound from the "AzureStorage" section of appsettings.json.
/// </summary>
public class StorageSettings
{
    /// <summary>The configuration section key used to bind this settings object.</summary>
    public const string SectionName = "AzureStorage";

    /// <summary>Azure Blob Storage connection string. Use "UseDevelopmentStorage=true" for Azurite locally.</summary>
    public string ConnectionString { get; init; } = default!;

    /// <summary>Name of the blob container where catalog images are stored. Defaults to "catalog-images".</summary>
    public string ContainerName { get; init; } = "catalog-images";
}

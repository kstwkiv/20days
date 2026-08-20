// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Infrastructure.Settings;

namespace NoCap.Eats.Catalog.Infrastructure.Services;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IImageStorageService"/>.
/// Uploads images to a public blob container and returns their CDN-accessible URLs.
/// Uses Azurite (UseDevelopmentStorage=true) when running locally.
/// </summary>
public class AzureBlobImageStorageService(
    BlobServiceClient         blobClient,
    IOptions<StorageSettings> opts) : IImageStorageService
{
    /// <summary>Resolved storage settings from configuration.</summary>
    private readonly StorageSettings _settings = opts.Value;

    /// <inheritdoc/>
    /// <remarks>
    /// Creates the container with public blob access if it does not exist.
    /// Prefixes the blob name with a new GUID to avoid filename collisions.
    /// </remarks>
    public async Task<string> UploadAsync(
        Stream imageStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var container = blobClient.GetBlobContainerClient(_settings.ContainerName);
        // Create the container with public read access if it doesn't exist yet
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        // Prefix with a GUID to ensure unique blob names even for identical file names
        var blobName = $"{Guid.NewGuid()}-{fileName}";
        var blob     = container.GetBlobClient(blobName);

        await blob.UploadAsync(imageStream,
            new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blob.Uri.ToString();
    }

    /// <inheritdoc/>
    /// <remarks>Silently succeeds if the blob does not exist (idempotent).</remarks>
    public async Task DeleteAsync(string imageUrl, CancellationToken ct = default)
    {
        var uri       = new Uri(imageUrl);
        var blobName  = uri.Segments.Last();
        var container = blobClient.GetBlobContainerClient(_settings.ContainerName);
        await container.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct);
    }
}

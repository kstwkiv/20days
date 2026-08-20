// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Application.Interfaces;

/// <summary>Abstraction over object storage for restaurant and menu item images.</summary>
public interface IImageStorageService
{
    /// <summary>Uploads an image stream to the configured storage provider and returns its public URL.</summary>
    /// <param name="imageStream">The image data to upload.</param>
    /// <param name="fileName">Original file name including extension.</param>
    /// <param name="contentType">MIME type of the image (e.g. "image/jpeg").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The publicly accessible URL of the uploaded image.</returns>
    Task<string> UploadAsync(Stream imageStream, string fileName, string contentType, CancellationToken ct = default);

    /// <summary>Deletes the image at the given URL from the storage provider.</summary>
    /// <param name="imageUrl">Public URL of the image to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(string imageUrl, CancellationToken ct = default);
}

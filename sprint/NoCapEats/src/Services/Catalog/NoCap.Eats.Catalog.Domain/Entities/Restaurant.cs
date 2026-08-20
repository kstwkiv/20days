// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Domain.Enums;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Domain.Entities;

/// <summary>
/// Aggregate root for a restaurant listing in the catalog.
/// Manages its own menu categories and enforces owner-based access control.
/// </summary>
public class Restaurant
{
    /// <summary>Unique identifier of this restaurant.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the RestaurantOwner user who owns this listing.</summary>
    public Guid OwnerId { get; private set; }

    /// <summary>Display name of the restaurant.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Marketing description shown to customers.</summary>
    public string Description { get; private set; } = default!;

    /// <summary>Street address of the restaurant.</summary>
    public string Address { get; private set; } = default!;

    /// <summary>City where the restaurant is located, used for filtering.</summary>
    public string City { get; private set; } = default!;

    /// <summary>Contact phone number of the restaurant.</summary>
    public string Phone { get; private set; } = default!;

    /// <summary>URL of the restaurant's cover image, or <c>null</c> if not set.</summary>
    public string? ImageUrl { get; private set; }

    /// <summary>Optional cuisine type label (e.g. "Italian", "American").</summary>
    public string? CuisineType { get; private set; }

    /// <summary>Current approval and operational status of this restaurant.</summary>
    public RestaurantStatus Status { get; private set; } = RestaurantStatus.PendingApproval;

    /// <summary>Indicates whether the restaurant is currently accepting orders.</summary>
    public bool IsOpen { get; private set; }

    /// <summary>UTC timestamp when this listing was created.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Backing field for menu categories.</summary>
    private readonly List<MenuCategory> _categories = [];

    /// <summary>Read-only view of the restaurant's menu categories.</summary>
    public IReadOnlyList<MenuCategory> Categories => _categories.AsReadOnly();

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected Restaurant() { }

    /// <summary>Creates a new restaurant listing in PendingApproval status.</summary>
    /// <param name="ownerId">ID of the RestaurantOwner user.</param>
    /// <param name="name">Display name of the restaurant.</param>
    /// <param name="description">Marketing description.</param>
    /// <param name="address">Street address.</param>
    /// <param name="city">City for search filtering.</param>
    /// <param name="phone">Contact phone number.</param>
    /// <param name="cuisineType">Optional cuisine type label.</param>
    public Restaurant(Guid ownerId, string name, string description,
                      string address, string city, string phone,
                      string? cuisineType = null)
    {
        OwnerId     = ownerId;
        Name        = name;
        Description = description;
        Address     = address;
        City        = city;
        Phone       = phone;
        CuisineType = cuisineType;
    }

    /// <summary>Updates the restaurant's editable details.</summary>
    public void Update(string name, string description, string address,
                       string city, string phone, string? cuisineType)
    {
        Name        = name;
        Description = description;
        Address     = address;
        City        = city;
        Phone       = phone;
        CuisineType = cuisineType;
        UpdatedAt   = DateTime.UtcNow;
    }

    /// <summary>Sets or replaces the restaurant's cover image URL.</summary>
    /// <param name="imageUrl">Public URL of the uploaded image.</param>
    public void SetImage(string imageUrl) { ImageUrl = imageUrl; UpdatedAt = DateTime.UtcNow; }

    /// <summary>Sets status to Active (admin-approved).</summary>
    public void Activate() { Status = RestaurantStatus.Active;    UpdatedAt = DateTime.UtcNow; }

    /// <summary>Sets status to Suspended (temporarily unavailable).</summary>
    public void Suspend()  { Status = RestaurantStatus.Suspended; UpdatedAt = DateTime.UtcNow; }

    /// <summary>Sets status to Closed (permanently closed).</summary>
    public void Close()    { Status = RestaurantStatus.Closed;    UpdatedAt = DateTime.UtcNow; }

    /// <summary>Marks the restaurant as currently accepting orders.</summary>
    public void OpenDoors()  { IsOpen = true;  UpdatedAt = DateTime.UtcNow; }

    /// <summary>Marks the restaurant as not accepting orders.</summary>
    public void CloseDoors() { IsOpen = false; UpdatedAt = DateTime.UtcNow; }

    /// <summary>Adds a new menu category to this restaurant and returns it.</summary>
    /// <param name="name">Display name of the category.</param>
    /// <param name="description">Optional description of the category.</param>
    /// <returns>The newly created <see cref="MenuCategory"/>.</returns>
    public MenuCategory AddCategory(string name, string? description = null)
    {
        var category = new MenuCategory(Id, name, description);
        _categories.Add(category);
        UpdatedAt = DateTime.UtcNow;
        return category;
    }

    /// <summary>
    /// Throws if the requesting user is not the owner of this restaurant.
    /// Used to enforce ownership before any write operation.
    /// </summary>
    /// <param name="requestingOwnerId">The ID of the user attempting the action.</param>
    /// <exception cref="UnauthorizedRestaurantAccessException">Thrown when IDs do not match.</exception>
    public void GuardOwner(Guid requestingOwnerId)
    {
        if (OwnerId != requestingOwnerId)
            throw new UnauthorizedRestaurantAccessException(requestingOwnerId, Id);
    }
}

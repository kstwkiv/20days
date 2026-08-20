// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoCap.Eats.Catalog.Application.Commands.MenuCategory.CreateMenuCategory;
using NoCap.Eats.Catalog.Application.Commands.MenuItem.CreateMenuItem;
using NoCap.Eats.Catalog.Application.Commands.MenuItem.UpdateMenuItem;
using NoCap.Eats.Catalog.Application.Commands.Restaurant.CreateRestaurant;
using NoCap.Eats.Catalog.Application.Commands.Restaurant.UpdateRestaurant;
using NoCap.Eats.Catalog.Application.Queries.GetMyRestaurants;
using NoCap.Eats.Catalog.Application.Queries.GetRestaurant;
using NoCap.Eats.Catalog.Application.Queries.ListRestaurants;
using System.Security.Claims;

namespace NoCap.Eats.Catalog.API.Endpoints;

public static class RestaurantEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        // ── Public (browse) ───────────────────────────────────────────────────
        var pub = app.MapGroup("/api/restaurants").WithTags("Restaurants");

        pub.MapGet("/", async ([FromQuery] string? city, ISender sender) =>
            Results.Ok(await sender.Send(new ListRestaurantsQuery(city))))
            .WithName("ListRestaurants")
            .WithSummary("Browse all active restaurants, optionally filtered by city.")
            .Produces(StatusCodes.Status200OK);

        pub.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new GetRestaurantQuery(id))))
            .WithName("GetRestaurant")
            .WithSummary("Get a restaurant with its full menu.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // ── Owner-only (manage) ───────────────────────────────────────────────
        var owner = app.MapGroup("/api/owner/restaurants")
            .WithTags("Owner- Restaurants")
            .RequireAuthorization("RestaurantOwner");

        owner.MapGet("/", async (HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            return Results.Ok(await sender.Send(new GetMyRestaurantsQuery(ownerId)));
        })
        .WithName("GetMyRestaurants")
        .WithSummary("List all restaurants belonging to the authenticated owner.")
        .Produces(StatusCodes.Status200OK);

        owner.MapPost("/", async (
            [FromBody] CreateRestaurantRequest req,
            HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            var result  = await sender.Send(new CreateRestaurantCommand(
                ownerId, req.Name, req.Description, req.Address,
                req.City, req.Phone, req.CuisineType));
            return Results.Created($"/api/restaurants/{result.Id}", result);
        })
        .WithName("CreateRestaurant")
        .WithSummary("Register a new restaurant.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        owner.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateRestaurantRequest req,
            HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            var result  = await sender.Send(new UpdateRestaurantCommand(
                id, ownerId, req.Name, req.Description, req.Address,
                req.City, req.Phone, req.CuisineType));
            return Results.Ok(result);
        })
        .WithName("UpdateRestaurant")
        .WithSummary("Update restaurant details.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // ── Menu Categories ───────────────────────────────────────────────────
        owner.MapPost("/{restaurantId:guid}/categories", async (
            Guid restaurantId,
            [FromBody] CreateMenuCategoryRequest req,
            HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            var result  = await sender.Send(new CreateMenuCategoryCommand(
                restaurantId, ownerId, req.Name, req.Description, req.SortOrder));
            return Results.Created($"/api/restaurants/{restaurantId}", result);
        })
        .WithName("CreateMenuCategory")
        .WithSummary("Add a menu category (e.g. Starters, Mains).")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // ── Menu Items ────────────────────────────────────────────────────────
        owner.MapPost("/{restaurantId:guid}/categories/{categoryId:guid}/items", async (
            Guid categoryId,
            [FromBody] CreateMenuItemRequest req,
            HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            var result  = await sender.Send(new CreateMenuItemCommand(
                categoryId, ownerId, req.Name, req.Description, req.Price));
            return Results.Created(string.Empty, result);
        })
        .WithName("CreateMenuItem")
        .WithSummary("Add a menu item to a category.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        owner.MapPut("/{restaurantId:guid}/items/{itemId:guid}", async (
            Guid itemId,
            [FromBody] UpdateMenuItemRequest req,
            HttpContext http, ISender sender) =>
        {
            var ownerId = GetUserId(http);
            var result  = await sender.Send(new UpdateMenuItemCommand(
                itemId, ownerId, req.Name, req.Description, req.Price, req.ImageUrl));
            return Results.Ok(result);
        })
        .WithName("UpdateMenuItem")
        .WithSummary("Update a menu item.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static Guid GetUserId(HttpContext http)
    {
        var value = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? http.User.FindFirstValue("sub");
        return Guid.Parse(value!);
    }
}

// ── Request models ────────────────────────────────────────────────────────────
public record CreateRestaurantRequest(
    string Name, string Description, string Address,
    string City, string Phone, string? CuisineType);

public record UpdateRestaurantRequest(
    string Name, string Description, string Address,
    string City, string Phone, string? CuisineType);

public record CreateMenuCategoryRequest(
    string Name, string? Description, int SortOrder = 0);

public record CreateMenuItemRequest(
    string Name, string Description, decimal Price);

public record UpdateMenuItemRequest(
    string Name, string Description, decimal Price, string? ImageUrl);

// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoCap.Eats.Order.Application.Commands.CancelOrder;
using NoCap.Eats.Order.Application.Commands.PlaceOrder;
using NoCap.Eats.Order.Application.Commands.UpdateOrderStatus;
using NoCap.Eats.Order.Application.Queries.GetMyOrders;
using NoCap.Eats.Order.Application.Queries.GetOrder;
using NoCap.Eats.Order.Application.Queries.GetRestaurantOrders;
using NoCap.Eats.Order.Domain.Enums;
using System.Security.Claims;

namespace NoCap.Eats.Order.API.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        // ── Customer endpoints ────────────────────────────────────────────────
        var customer = app.MapGroup("/api/orders")
            .WithTags("Orders — Customer")
            .RequireAuthorization();

        customer.MapPost("/", async (
            [FromBody] PlaceOrderRequest req,
            HttpContext http, ISender sender) =>
        {
            var customerId = GetUserId(http);
            var result     = await sender.Send(new PlaceOrderCommand(
                customerId,
                req.RestaurantId,
                req.CustomerName,
                req.CustomerPhone,
                req.DeliveryAddress,
                req.DeliveryNotes,
                req.Items.Select(i => new PlaceOrderCommand.OrderLine(
                    i.MenuItemId, i.Name, i.Quantity, i.UnitPrice)).ToList()));

            return Results.Created($"/api/orders/{result.Id}", result);
        })
        .WithName("PlaceOrder")
        .WithSummary("Place a new order.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        customer.MapGet("/my", async (HttpContext http, ISender sender) =>
        {
            var customerId = GetUserId(http);
            return Results.Ok(await sender.Send(new GetMyOrdersQuery(customerId)));
        })
        .WithName("GetMyOrders")
        .WithSummary("List all orders for the authenticated customer.")
        .Produces(StatusCodes.Status200OK);

        customer.MapGet("/{id:guid}", async (
            Guid id, HttpContext http, ISender sender) =>
        {
            var requesterId = GetUserId(http);
            return Results.Ok(await sender.Send(new GetOrderQuery(id, requesterId)));
        })
        .WithName("GetOrder")
        .WithSummary("Get a single order with its items.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        customer.MapPost("/{id:guid}/cancel", async (
            Guid id, HttpContext http, ISender sender) =>
        {
            var customerId = GetUserId(http);
            var result     = await sender.Send(new CancelOrderCommand(id, customerId));
            return Results.Ok(result);
        })
        .WithName("CancelOrder")
        .WithSummary("Cancel a Pending or Confirmed order.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status409Conflict);

        // ── Restaurant owner endpoints ─────────────────────────────────────────
        var owner = app.MapGroup("/api/restaurant-orders")
            .WithTags("Orders — Restaurant")
            .RequireAuthorization("RestaurantOwner");

        owner.MapGet("/{restaurantId:guid}", async (
            Guid restaurantId, ISender sender) =>
            Results.Ok(await sender.Send(new GetRestaurantOrdersQuery(restaurantId))))
        .WithName("GetRestaurantOrders")
        .WithSummary("List all orders for a restaurant.")
        .Produces(StatusCodes.Status200OK);

        owner.MapPut("/{orderId:guid}/status", async (
            Guid orderId,
            [FromBody] UpdateOrderStatusRequest req,
            HttpContext http, ISender sender) =>
        {
            var requesterId = GetUserId(http);
            var result      = await sender.Send(
                new UpdateOrderStatusCommand(orderId, requesterId, req.Status));
            return Results.Ok(result);
        })
        .WithName("UpdateOrderStatus")
        .WithSummary("Drive an order through its lifecycle (Confirm → Preparing → ReadyForPickup → etc.).")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status409Conflict)
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
public record PlaceOrderRequest(
    Guid   RestaurantId,
    string CustomerName,
    string CustomerPhone,
    string DeliveryAddress,
    string? DeliveryNotes,
    IReadOnlyList<PlaceOrderRequest.LineItem> Items)
{
    public record LineItem(Guid MenuItemId, string Name, int Quantity, decimal UnitPrice);
}

public record UpdateOrderStatusRequest(OrderStatus Status);

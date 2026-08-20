// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoCap.Eats.Delivery.Application.Commands.AcceptDelivery;
using NoCap.Eats.Delivery.Application.Commands.UpdateDeliveryStatus;
using NoCap.Eats.Delivery.Application.Queries.GetMyDeliveries;
using NoCap.Eats.Delivery.Application.Queries.GetPendingDeliveries;
using NoCap.Eats.Delivery.Domain.Enums;
using System.Security.Claims;

namespace NoCap.Eats.Delivery.API.Endpoints;

public static class DeliveryEndpoints
{
    public static IEndpointRouteBuilder MapDeliveryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/deliveries")
            .WithTags("Deliveries")
            .RequireAuthorization("DeliveryAgent");

        // List available (pending) deliveries for agents to claim
        group.MapGet("/pending", async (ISender sender) =>
            Results.Ok(await sender.Send(new GetPendingDeliveriesQuery())))
        .WithName("GetPendingDeliveries")
        .WithSummary("List all orders ready for a delivery agent to accept.")
        .Produces(StatusCodes.Status200OK);

        // Agent's own active/past deliveries
        group.MapGet("/my", async (HttpContext http, ISender sender) =>
        {
            var agentId = GetUserId(http);
            return Results.Ok(await sender.Send(new GetMyDeliveriesQuery(agentId)));
        })
        .WithName("GetMyDeliveries")
        .WithSummary("List all deliveries assigned to the authenticated agent.")
        .Produces(StatusCodes.Status200OK);

        // Self-assign a pending delivery
        group.MapPost("/{id:guid}/accept", async (
            Guid id, HttpContext http, ISender sender) =>
        {
            var agentId = GetUserId(http);
            var result  = await sender.Send(new AcceptDeliveryCommand(id, agentId));
            return Results.Ok(result);
        })
        .WithName("AcceptDelivery")
        .WithSummary("Claim a pending delivery.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status404NotFound);

        // Drive delivery status: PickedUp → Delivered / Failed
        group.MapPut("/{id:guid}/status", async (
            Guid id,
            [FromBody] UpdateDeliveryStatusRequest req,
            HttpContext http, ISender sender) =>
        {
            var agentId = GetUserId(http);
            var result  = await sender.Send(
                new UpdateDeliveryStatusCommand(id, agentId, req.Status));
            return Results.Ok(result);
        })
        .WithName("UpdateDeliveryStatus")
        .WithSummary("Update delivery status (PickedUp, Delivered, Failed).")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status409Conflict);

        return app;
    }

    private static Guid GetUserId(HttpContext http)
    {
        var value = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? http.User.FindFirstValue("sub");
        return Guid.Parse(value!);
    }
}

public record UpdateDeliveryStatusRequest(DeliveryStatus Status);

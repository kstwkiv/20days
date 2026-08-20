// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.Extensions.Logging;
using NoCap.Eats.BuildingBlocks.Events;

namespace NoCap.Eats.Catalog.Infrastructure.Messaging.Consumers;

/// <summary>
/// Listens for new user registrations.
/// For RestaurantOwner registrations, this is where you'd create
/// an owner profile or trigger any catalog-side onboarding logic.
/// </summary>
public class UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger)
    : IConsumer<UserRegisteredEvent>
{
    public Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var ev = context.Message;

        logger.LogInformation(
            "Catalog received UserRegisteredEvent: UserId={UserId} Role={Role}",
            ev.UserId, ev.Role);

        // Restaurant owners are ready to create restaurants via the API.
        // If future logic is needed (e.g. creating an OwnerProfile aggregate),
        // add it here.

        return Task.CompletedTask;
    }
}

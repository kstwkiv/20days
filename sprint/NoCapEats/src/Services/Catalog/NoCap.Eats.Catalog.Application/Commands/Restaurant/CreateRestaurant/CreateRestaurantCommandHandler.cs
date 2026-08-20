// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using RestaurantEntity = NoCap.Eats.Catalog.Domain.Entities.Restaurant;

namespace NoCap.Eats.Catalog.Application.Commands.Restaurant.CreateRestaurant;

/// <summary>
/// Handles <see cref="CreateRestaurantCommand"/> by instantiating a new restaurant aggregate
/// in PendingApproval status and persisting it.
/// </summary>
public class CreateRestaurantCommandHandler(
    IRestaurantRepository repo) : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
{
    /// <summary>Creates and saves the new restaurant for the requesting owner.</summary>
    /// <param name="request">Restaurant details provided by the owner.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RestaurantDto"/> representing the newly created restaurant.</returns>
    public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = new RestaurantEntity(
            request.OwnerId,
            request.Name,
            request.Description,
            request.Address,
            request.City,
            request.Phone,
            request.CuisineType);

        await repo.AddAsync(restaurant, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);

        return restaurant.ToDto();
    }
}

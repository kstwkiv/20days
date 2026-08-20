// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Application.Commands.Restaurant.UpdateRestaurant;

/// <summary>
/// Handles <see cref="UpdateRestaurantCommand"/> by verifying ownership
/// and applying the updated details to the restaurant aggregate.
/// </summary>
public class UpdateRestaurantCommandHandler(
    IRestaurantRepository repo) : IRequestHandler<UpdateRestaurantCommand, RestaurantDto>
{
    /// <summary>Updates the restaurant details after verifying the requesting user owns it.</summary>
    /// <param name="request">Update details including the restaurant ID and new values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="RestaurantDto"/>.</returns>
    /// <exception cref="RestaurantNotFoundException">Thrown when the restaurant does not exist.</exception>
    /// <exception cref="UnauthorizedRestaurantAccessException">Thrown when the requester is not the owner.</exception>
    public async Task<RestaurantDto> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await repo.GetByIdAsync(request.RestaurantId, cancellationToken)
            ?? throw new RestaurantNotFoundException(request.RestaurantId);

        restaurant.GuardOwner(request.OwnerId);
        restaurant.Update(request.Name, request.Description, request.Address,
                          request.City, request.Phone, request.CuisineType);

        await repo.SaveChangesAsync(cancellationToken);
        return restaurant.ToDto();
    }
}

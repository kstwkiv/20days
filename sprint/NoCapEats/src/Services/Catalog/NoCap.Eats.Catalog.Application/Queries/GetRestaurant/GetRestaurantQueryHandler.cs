// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Application.Queries.GetRestaurant;

/// <summary>
/// Handles <see cref="GetRestaurantQuery"/> by loading the full restaurant
/// including its categories and menu items.
/// </summary>
public class GetRestaurantQueryHandler(
    IRestaurantRepository repo) : IRequestHandler<GetRestaurantQuery, RestaurantDetailDto>
{
    /// <summary>Fetches and returns the restaurant with its complete menu.</summary>
    /// <param name="request">Query containing the restaurant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RestaurantDetailDto"/> with all categories and items.</returns>
    /// <exception cref="RestaurantNotFoundException">Thrown when the restaurant does not exist.</exception>
    public async Task<RestaurantDetailDto> Handle(GetRestaurantQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await repo.GetByIdWithCategoriesAsync(request.RestaurantId, cancellationToken)
            ?? throw new RestaurantNotFoundException(request.RestaurantId);

        return restaurant.ToDetailDto();
    }
}

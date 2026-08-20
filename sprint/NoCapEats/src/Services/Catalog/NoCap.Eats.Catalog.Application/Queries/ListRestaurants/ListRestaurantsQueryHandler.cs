// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;

namespace NoCap.Eats.Catalog.Application.Queries.ListRestaurants;

/// <summary>Handles <see cref="ListRestaurantsQuery"/> by returning all Active restaurants.</summary>
public class ListRestaurantsQueryHandler(
    IRestaurantRepository repo) : IRequestHandler<ListRestaurantsQuery, IReadOnlyList<RestaurantDto>>
{
    /// <summary>Returns active restaurants, optionally filtered by city.</summary>
    /// <param name="request">Query containing an optional city filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="RestaurantDto"/> records ordered by name.</returns>
    public async Task<IReadOnlyList<RestaurantDto>> Handle(
        ListRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await repo.GetAllActiveAsync(request.City, cancellationToken);
        return restaurants.Select(r => r.ToDto()).ToList();
    }
}

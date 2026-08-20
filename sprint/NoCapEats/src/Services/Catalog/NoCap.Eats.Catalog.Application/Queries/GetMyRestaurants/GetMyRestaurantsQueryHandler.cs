// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;

namespace NoCap.Eats.Catalog.Application.Queries.GetMyRestaurants;

/// <summary>Handles <see cref="GetMyRestaurantsQuery"/> by returning all restaurants owned by the requesting user.</summary>
public class GetMyRestaurantsQueryHandler(
    IRestaurantRepository repo) : IRequestHandler<GetMyRestaurantsQuery, IReadOnlyList<RestaurantDto>>
{
    /// <summary>Returns all restaurants for the authenticated owner regardless of status.</summary>
    /// <param name="request">Query containing the owner ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="RestaurantDto"/> records for the owner.</returns>
    public async Task<IReadOnlyList<RestaurantDto>> Handle(
        GetMyRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await repo.GetByOwnerAsync(request.OwnerId, cancellationToken);
        return restaurants.Select(r => r.ToDto()).ToList();
    }
}

// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Queries.GetCurrentUser;

/// <summary>
/// Query to retrieve the profile of the currently authenticated user.
/// The user ID is extracted from the JWT claim in the endpoint layer.
/// </summary>
/// <param name="UserId">ID of the authenticated user to fetch.</param>
public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;

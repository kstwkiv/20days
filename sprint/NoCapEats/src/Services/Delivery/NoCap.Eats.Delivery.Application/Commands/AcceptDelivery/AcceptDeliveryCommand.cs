// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;

namespace NoCap.Eats.Delivery.Application.Commands.AcceptDelivery;

/// <summary>Agent self-assigns a pending delivery.</summary>
public record AcceptDeliveryCommand(Guid DeliveryId, Guid AgentId) : IRequest<DeliveryDto>;

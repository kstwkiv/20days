// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Domain.Exceptions;

/// <summary>Base exception for all domain-level errors in the Identity service.</summary>
/// <param name="message">Human-readable description of the error.</param>
public class DomainException(string message) : Exception(message);

/// <summary>Thrown when a user with the specified ID cannot be found.</summary>
/// <param name="userId">The ID that was searched for.</param>
public class UserNotFoundException(Guid userId)
    : DomainException($"User '{userId}' was not found.");

/// <summary>Thrown when attempting to register an email address that already exists.</summary>
/// <param name="email">The duplicate email address.</param>
public class EmailAlreadyRegisteredException(string email)
    : DomainException($"Email '{email}' is already registered.");

/// <summary>Thrown when a login attempt fails due to incorrect email or password.</summary>
public class InvalidCredentialsException()
    : DomainException("Invalid email or password.");

/// <summary>Thrown when a login or action is attempted on a deactivated account.</summary>
public class AccountDeactivatedException()
    : DomainException("This account has been deactivated.");

/// <summary>Thrown when a refresh token is missing, expired, or already revoked.</summary>
public class InvalidRefreshTokenException()
    : DomainException("The refresh token is invalid or has expired.");

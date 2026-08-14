namespace NoCap.Eats.Identity.Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class UserNotFoundException(Guid userId)
    : DomainException($"User '{userId}' was not found.");

public class EmailAlreadyRegisteredException(string email)
    : DomainException($"Email '{email}' is already registered.");

public class InvalidCredentialsException()
    : DomainException("Invalid email or password.");

public class AccountDeactivatedException()
    : DomainException("This account has been deactivated.");

public class InvalidRefreshTokenException()
    : DomainException("The refresh token is invalid or has expired.");

using Dsw2025Tpi.Application.Dtos.Users;

namespace Dsw2025Tpi.Application.Interfaces;

/// <summary>
/// Service for managing user authentication and registration operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The unique identifier of the newly created user.</returns>
    /// <exception cref="Domain.Exceptions.AuthExceptions.UserCreationFailedException">Thrown when user creation fails.</exception>
    Task<string> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns their login information.
    /// </summary>
    /// <param name="request">The login request containing credentials.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A tuple containing the username, role, and optional customer ID.</returns>
    /// <exception cref="Domain.Exceptions.AuthExceptions.InvalidCredentialsException">Thrown when credentials are invalid.</exception>
    Task<(string Username, string Role, Guid? CustomerId)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

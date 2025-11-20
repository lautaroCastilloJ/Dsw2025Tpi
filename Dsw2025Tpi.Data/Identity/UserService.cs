using Dsw2025Tpi.Application.Dtos.Users;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Exceptions.AuthExceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Data.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting registration for user: {Username}, Email: {Email}", 
            request.UserName, request.Email);
        
        string userId = string.Empty;

        try
        {
            await _unitOfWork.ExecuteAsync(async () =>
            {
                // 1. Crear AppUser
                var user = new AppUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    DisplayName = request.DisplayName
                };

                _logger.LogDebug("Creating user in Identity system");
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    _logger.LogWarning("User creation failed: {Errors}", errors);
                    throw new UserCreationFailedException(errors);
                }

                userId = user.Id;
                _logger.LogDebug("User created with ID: {UserId}", userId);

                // Asignar rol
                var role = string.IsNullOrWhiteSpace(request.Role) ? AppRoles.Cliente : request.Role;
                _logger.LogDebug("Assigning role: {Role}", role);
                
                // Crear el rol si no existe
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation("Role {Role} does not exist, creating it", role);
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(" | ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    _logger.LogWarning("Role assignment failed: {Errors}", roleErrors);
                    throw new UserCreationFailedException(roleErrors);
                }

                _logger.LogDebug("Role assigned successfully");

                // 2. Crear Customer vinculado al AppUser (solo para clientes)
                if (role.Equals(AppRoles.Cliente, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Creating Customer entity for client user");
                    
                    var customer = Customer.Create(
                        request.Email,
                        request.DisplayName,
                        request.PhoneNumber
                    );

                    await _customerRepository.Add(customer);
                    _logger.LogDebug("Customer created with ID: {CustomerId}", customer.Id);

                    // 3. Asignar CustomerId al AppUser
                    user.CustomerId = customer.Id;
                    user.PhoneNumber = request.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                    
                    _logger.LogDebug("User updated with CustomerId");
                }

            }, cancellationToken);

            _logger.LogInformation("Registration completed successfully for user: {Username}", request.UserName);
            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for user: {Username}", request.UserName);
            throw;
        }
    }

    public async Task<(string Username, string Role, Guid? CustomerId)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Login failed: Empty credentials");
                throw new InvalidCredentialsException();
            }

            _logger.LogDebug("Attempting password sign-in");
            var result = await _signInManager.PasswordSignInAsync(
                request.Username, request.Password, false, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid credentials for user {Username}", request.Username);
                throw new InvalidCredentialsException();
            }

            _logger.LogDebug("Password sign-in successful, fetching user details");
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found after successful sign-in");
                throw new InvalidCredentialsException();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? AppRoles.Cliente;

            _logger.LogInformation("Login successful for user: {Username}, Role: {Role}, CustomerId: {CustomerId}", 
                request.Username, userRole, user.CustomerId);

            return (request.Username, userRole, user.CustomerId);
        }
        catch (Exception ex) when (ex is not InvalidCredentialsException)
        {
            _logger.LogError(ex, "Unexpected error during login for user: {Username}", request.Username);
            throw;
        }
    }
}

using Dsw2025Tpi.Application.Dtos.Users;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Exceptions.AuthExceptions;
using Microsoft.AspNetCore.Identity;

namespace Dsw2025Tpi.Data.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        string userId = string.Empty;

        await _unitOfWork.ExecuteAsync(async () =>
        {
            // 1. Crear AppUser
            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Log detallado de los errores
                var errors = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new UserCreationFailedException(errors);
            }

            userId = user.Id;

            // Asignar rol (usar AppRoles para consistencia)
            var role = string.IsNullOrWhiteSpace(request.Role) ? AppRoles.Cliente : request.Role;
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(" | ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new UserCreationFailedException(roleErrors);
            }

            // 2. Crear Customer vinculado al AppUser (solo para clientes)
            if (role.Equals(AppRoles.Cliente, StringComparison.OrdinalIgnoreCase))
            {
                var customer = Customer.Create(
                    request.Email,
                    request.DisplayName,
                    request.PhoneNumber
                );

                await _customerRepository.Add(customer);

                // 3. Asignar CustomerId al AppUser
                user.CustomerId = customer.Id;
                user.PhoneNumber = request.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

        });

        return userId;
    }

    public async Task<(string Username, string Role)> LoginAsync(LoginRequest request)
    {
        ValidateLoginRequest(request);

        var result = await _signInManager.PasswordSignInAsync(
            request.Username, request.Password, false, false);

        if (!result.Succeeded)
            throw new InvalidCredentialsException();

        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new InvalidCredentialsException();

        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? AppRoles.Cliente;

        return (request.Username, userRole);
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidCredentialsException();
    }
}


using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;



namespace Dsw2025Tpi.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterModelRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.UserName) || request.UserName.Length < 3)
            throw new ArgumentException("El nombre de usuario (UserName) debe tener al menos 3 caracteres.");

        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length < 3)
            throw new ArgumentException("El nombre visible (DisplayName) debe tener al menos 3 caracteres.");

        if (!new EmailAddressAttribute().IsValid(request.Email))
            throw new ArgumentException("El email ingresado no tiene un formato válido.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ArgumentException("El email ya está asociado a un Usuario. Por favor, elija otro.");

        var role = AppRoles.Cliente;

        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName
            //EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return result;

        // Crear rol "Cliente" si no existe
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        // Asignar el rol por defecto
        await _userManager.AddToRoleAsync(user, role);

        return result;
    }


    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("Usuario no encontrado.");

        return (List<string>)await _userManager.GetRolesAsync(user);
    }

    public async Task<(AppUser User, string Role)> ValidateUserAsync(LoginModelRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            // Revisión extra: ¿existe con ese email pero username distinto?
            var byEmail = await _userManager.FindByEmailAsync(request.Username);
            if (byEmail != null)
            {
                var passOk = await _signInManager.CheckPasswordSignInAsync(byEmail, request.Password, false);
                if (passOk.Succeeded)
                    throw new UnauthorizedAccessException("Nombre de usuario incorrecto.");
            }

            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Contraseña incorrecta.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault(r => AppRoles.AllowedRoles.Contains(r));
        if (role is null)
            throw new UnauthorizedAccessException("El usuario no tiene un rol válido asignado.");

        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new InvalidOperationException("El usuario no tiene un nombre de usuario asignado.");

        return (user, role);
    }

    public async Task<IdentityResult> InitializeAdminAsync(SetupAdminRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Ya existe un usuario con ese correo electrónico."
            });
        }

        if (string.IsNullOrWhiteSpace(request.UserName) || request.UserName.Length < 3)
            throw new ArgumentException("El nombre de usuario (UserName) debe tener al menos 3 caracteres.");

        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length < 3)
            throw new ArgumentException("El nombre visible (DisplayName) debe tener al menos 3 caracteres.");

        if (!new EmailAddressAttribute().IsValid(request.Email))
            throw new ArgumentException("El email ingresado no tiene un formato válido.");

        var newUser = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName
            //EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
            return result;

        // Asegurar que el rol Administrador exista
        if (!await _roleManager.RoleExistsAsync(AppRoles.Administrador))
            await _roleManager.CreateAsync(new IdentityRole(AppRoles.Administrador));

        await _userManager.AddToRoleAsync(newUser, AppRoles.Administrador);

        return IdentityResult.Success;
    }


}

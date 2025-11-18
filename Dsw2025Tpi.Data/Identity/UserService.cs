using Dsw2025Tpi.Application.Dtos.Users;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Dsw2025Tpi.Data.Identity;

namespace Dsw2025Tpi.Data.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new Exception("Error al crear usuario."); // luego usarás excepciones personalizadas

        return user.Id;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(
            request.Username, request.Password, false, false);

        if (!result.Succeeded)
            throw new Exception("Credenciales inválidas.");

        return request.Username;
    }
}
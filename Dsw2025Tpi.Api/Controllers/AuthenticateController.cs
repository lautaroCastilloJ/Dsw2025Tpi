using Dsw2025Tpi.Application.Dtos.Users;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticateController(IUserService userService, IJwtTokenService jwtTokenService)
    {
        _userService = userService;
        _jwtTokenService = jwtTokenService;
    }

    // ========= LOGIN (único endpoint) =========
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (username, role) = await _userService.LoginAsync(request);

        var token = _jwtTokenService.GenerateToken(username, role);

        return Ok(new
        {
            token,
            role
        });
    }

    // ========= REGISTER (único endpoint) =========
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Aquí delegás TODO al servicio: rol por defecto, creación de AppUser, Customer, etc.
        var userId = await _userService.RegisterAsync(request);

        return Ok(new
        {
            userId,
            message = "Usuario registrado correctamente."
        });
    }
}

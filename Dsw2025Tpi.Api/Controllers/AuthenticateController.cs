using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IUserService _userService;

    public AuthenticateController(JwtTokenService jwtTokenService, IUserService userService)
    {
        _jwtTokenService = jwtTokenService;
        _userService = userService;
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModelRequest request)
    {
        var (user, role) = await _userService.ValidateUserAsync(request);
        var token = _jwtTokenService.GenerateToken(user.UserName!, role);
        return Ok(new { token });
    }



    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
    {
        var result = await _userService.RegisterUserAsync(model);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = errors });
        }

        return Ok($"Usuario registrado correctamente con rol Cliente");
    }



}

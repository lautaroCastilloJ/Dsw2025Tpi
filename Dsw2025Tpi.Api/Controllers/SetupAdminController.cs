using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/setup")]
public class SetupAdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public SetupAdminController(IUserService userService,
                           UserManager<AppUser> userManager,
                           RoleManager<IdentityRole> roleManager)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> InitializeAdmin([FromBody] SetupAdminRequest request)
    {

        var admins = await _userManager.GetUsersInRoleAsync(AppRoles.Administrador);

        if (admins.Any())
        {
            return Forbid("Ya existe un administrador en el sistema. Este endpoint está deshabilitado.");
        }

        var result = await _userService.InitializeAdminAsync(request);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }

        return Ok("Administrador creado correctamente.");
    }
}


using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Data.Identity;
using Microsoft.AspNetCore.Identity;


namespace Dsw2025Tpi.Application.Interfaces;

public interface IUserService
{
    Task<IdentityResult> RegisterUserAsync(RegisterModelRequest request);
    Task<IdentityResult> InitializeAdminAsync(SetupAdminRequest request);
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<(AppUser User, string Role)> ValidateUserAsync(LoginModelRequest request);


}

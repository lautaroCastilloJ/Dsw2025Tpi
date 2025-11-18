using Dsw2025Tpi.Application.Dtos.Users;


namespace Dsw2025Tpi.Application.Interfaces;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
}

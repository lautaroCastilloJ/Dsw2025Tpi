using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Data.Persistence;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ========= DbContext Configuration =========
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<Dsw2025TpiContext>(options =>
            options.UseSqlServer(connectionString));

        // ========= Application Services =========
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();

        // ========= Repository & UnitOfWork =========
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

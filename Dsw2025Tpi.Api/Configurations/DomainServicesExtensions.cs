using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api.Configurations;

public static class DomainServicesConfigurationExtension
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.UseSeeding((c, t) =>
            {
                ((Dsw2025TpiContext)c).Seedwork<Customer>("Sources\\customers.json");
            });
        });

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();


        return services;
    }
}

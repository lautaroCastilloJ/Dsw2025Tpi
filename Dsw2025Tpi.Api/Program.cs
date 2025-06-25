using Dsw2025Tpi.Domain.Interfaces.Repositories;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Application.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using Dsw2025Tpi.Application.Services.Interfaces;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repositorios y UnitOfWork
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios de aplicación (los crearás en la siguiente capa)
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddAutoMapper(typeof(Dsw2025Tpi.Application.Mappings.MappingProfiles));



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<Dsw2025Tpi.Api.Middlewares.ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        // ---- AQUI CARGA LOS CUSTOMERS ----
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
            context.Database.Migrate(); // Aplica migraciones

            var customersJsonPath = Path.Combine(AppContext.BaseDirectory, "customers.json");
            context.SeedCustomers(customersJsonPath); // Carga los customers si faltan
        }

        // ---- FIN BLOQUE DE SEED ----

        app.Run();
    }
}

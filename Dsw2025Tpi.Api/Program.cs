using Dsw2025Tpi.Api.Middlewares;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Mappings;
using Dsw2025Tpi.Application.Services;

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

        // Servicios personalizados de la aplicación
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();



        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfiles));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}

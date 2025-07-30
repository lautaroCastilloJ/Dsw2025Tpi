using Dsw2025Tpi.Api.Configurations;
using Dsw2025Tpi.Api.Middlewares;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Validators;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

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
        
        //builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        //builder.Services.AddScoped<IProductService, ProductService>();
        //builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddDomainServices(builder.Configuration);
        builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<OrderItemRequestValidator>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddAutoMapper(typeof(Dsw2025Tpi.Application.Mappings.MappingProfiles));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");


        app.Run();
    }
}

namespace Ucms.Api.Extensions;

using System.Text.Json;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ucms.Application.Features;
using Ucms.Application.MappingProfiles;
using Ucms.Api.Middlewares;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDbContext<TContext>(
        this WebApplication app,
        Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        seeder(context, scope.ServiceProvider);
        return app;
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        builder.Services.AddUcmsDbContext(connectionString);
        builder.Services.AddAppIdentity();
        builder.Services.AddUcmsTokenService();
        builder.Services.AddUcmsCors("StockCors");
        builder.Services.AddApplicationAuth(builder.Configuration);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Application.Abstractions.ICurrentContext, Infrastructure.Services.HttpCurrentContext>();
        builder.Services.AddUcmsInfrastructureServices();
        builder.Services.AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true);
        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MeasurementUnitProfile).Assembly));
        builder.Services.AddFeatureHandlers();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddUcmsSwagger(builder.Configuration);
        builder.Services.AddApplicationHealthChecks();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<GlobalMiddlewareErrorHander>();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("StockCors");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UCMS API v1");
            c.DocumentTitle = "UCMS API";
            c.InjectJavascript("/swagger-custom.js");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapApplicationHealthChecks();
        app.MapControllers();

        // Swagger uchun custom login sahifasi
        app.MapGet("/auth/login", (IWebHostEnvironment env) =>
            Results.File(
                Path.Combine(env.WebRootPath, "auth", "login.html"),
                "text/html"))
            .ExcludeFromDescription();

        return app;
    }
}

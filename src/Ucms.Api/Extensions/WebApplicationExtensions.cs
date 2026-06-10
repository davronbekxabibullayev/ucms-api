namespace Ucms.Api.Extensions;

using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ucms.Application.MappingProfiles;
using Ucms.Api.Middlewares;
using Ucms.Application.Validators.Skus;

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
        builder.Services.AddAppDbContext(connectionString);
        builder.Services.AddUcmsMediator();
        builder.Services.AddUcmsCors("StockCors");
        builder.Services.AddUcmsServices();
        builder.Services.AddApplicationAuth();
        builder.Services
            .AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true)
            .AddValidatorsFromAssemblyContaining<CreateSkuRequestValidator>();
        builder.Services.AddAutoMapper(a => a.AddProfile<IncomeProfile>());

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
        app.UseRouting();
        app.UseCors("StockCors");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ucms.Api v1");
            c.OAuthClientId("Ucms_swaggerui");
            c.OAuthAppName("Gateway Swagger UI");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapApplicationHealthChecks();
        app.MapControllers();

        return app;
    }
}

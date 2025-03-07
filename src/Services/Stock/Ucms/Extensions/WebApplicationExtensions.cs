namespace Ucms.Stock.Api.Extensions;

using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Ucms.Stock.Api.Application.MappingProfiles;
using Ucms.Stock.Api.Middlewares;
using Ucms.Stock.Api.Validators.Skus;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        builder.Services.AddStockDbContext(connectionString);
        builder.Services.AddUcmsMediator();
        builder.Services.AddUcmsCors("StockCors");
        builder.Services.AddUcmsServices();
        builder.Services.AddApplicationAuth();
        builder.Services
            .AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true)
            .AddValidatorsFromAssemblyContaining<CreateSkuRequestValidator>();
        builder.Services.AddAutoMapper(typeof(IncomeProfile));
        builder.Services.ConfigureUcmsRabbitMq();

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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ucms.Stock.Api v1");
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

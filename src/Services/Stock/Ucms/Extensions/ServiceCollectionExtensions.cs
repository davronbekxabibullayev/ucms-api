namespace Ucms.Stock.Api.Extensions;

using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Filters;
using Ucms.Stock.Api.Application.Services;
using Ucms.Stock.Infrastructure.EntityFramework;
using Ucms.Stock.Infrastructure.Persistance;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStockDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StockDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        optionsBuilder.EnableRetryOnFailure(maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    })
                .EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        services.AddScoped<IStockDbContext, StockDbContext>();

        return services;
    }

    public static IServiceCollection AddUcmsMediator(this IServiceCollection services)
    {
        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(Assembly.GetExecutingAssembly());
        });
        return services;
    }

    public static IServiceCollection AddUcmsServices(this IServiceCollection services)
    {
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<IOutcomeService, OutcomeService>();

        return services;
    }

    public static IServiceCollection AddUcmsCors(this IServiceCollection services, string policyName)
    {
        services.AddCors(builder =>
        {
            builder.AddPolicy(policyName, options =>
                options
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        return services;
    }
}

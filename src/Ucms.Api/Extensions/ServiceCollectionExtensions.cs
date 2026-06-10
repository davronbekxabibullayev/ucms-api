namespace Ucms.Api.Extensions;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Services;
using Ucms.Application.Handlers;
using Ucms.Application.Persistence;
using Ucms.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                        optionsBuilder.EnableRetryOnFailure(maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    })
                .EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        services.AddScoped<IAppDbContext, AppDbContext>();

        return services;
    }

    public static IServiceCollection AddUcmsMediator(this IServiceCollection services)
    {
        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(typeof(IApplicationAssemblyMarker).Assembly);
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

namespace Ucms.Stock.Api.Extensions;

using Application.Consumers.Income;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Options;

public static class EventBusConfiguration
{
    public static IServiceCollection ConfigureUcmsRabbitMq(this IServiceCollection services)
    {
        services.AddEventBusOptions();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<CreateIncomeConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<EventBusOptions>>().Value;
                var uri = new Uri(options.Uri);

                cfg.Host(uri, h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });

                cfg.ReceiveEndpoint("create-income", c => { c.ConfigureConsumer<CreateIncomeConsumer>(context); });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddEventBusOptions(this IServiceCollection services, Action<EventBusOptions>? configureOptions = null)
    {
        var options = new EventBusOptions();
        configureOptions?.Invoke(options);

        services.AddOptions<EventBusOptions>()
            .BindConfiguration(EventBusOptions.SectionName)
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<EventBusOptions>, EventBusOptionsValidator>();

        return services;
    }
}

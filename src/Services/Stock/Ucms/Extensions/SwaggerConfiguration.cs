namespace Ucms.Stock.Api.Extensions;

using Microsoft.OpenApi.Models;
using System.Reflection;
using Ucms.Core.Filters;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddUcmsSwagger(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Ucms.Stock.Api",
                Description = "An ASP.NET Core Web API for managing Ucms.Stock.Api items",
                TermsOfService = new Uri("http://localhost:8211/stock-service")
            });

            options.OperationFilter<SwaggerOperationIdFilter>();

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{config["Identity:Url"]}/connect/authorize"),
                        TokenUrl = new Uri($"{config["Identity:Url"]}/connect/token"),
                        Scopes = new Dictionary<string, string>()
                        {
                            {
                                "stock_read", "Stock Api Read Access"
                            },
                            {
                                "stock_write", "Stock Api Write Access"
                            }
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }
}

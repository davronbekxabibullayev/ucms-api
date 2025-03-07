namespace Ucms.Stock.Api.Extensions;

using Devhub.Authorization.Extensions;

public static class PermissionsConfiguration
{
    public static IServiceCollection AddApplicationAuth(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddApplicationPermissions();

        return services;
    }

    public static IServiceCollection AddApplicationPermissions(this IServiceCollection services)
    {
        services
            .AddPermissions()
            .AddHttpPermissionProvider();

        return services;
    }
}

namespace Ucms.Api.Extensions;

public static class PermissionsConfiguration
{
    public static IServiceCollection AddApplicationAuth(this IServiceCollection services)
    {
        services.AddAuthentication();

        return services;
    }

}

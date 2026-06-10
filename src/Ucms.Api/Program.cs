using Microsoft.EntityFrameworkCore;
using Ucms.Api.Extensions;
using Ucms.Infrastructure.Persistence;

Console.Title = "UCMS STOCK API";


var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(SetupConfiguration());
builder.Host.UseApplicationSerilog();

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.MigrateDbContext<AppDbContext>((context, services) =>
{
    context.Database.Migrate();

    var config = services.GetRequiredService<IConfiguration>();
    if (config.GetValue<bool>("Database:EnabledDataSeeding"))
    {
        new AppDbContextSeed()
            .SeedAsync(context, services)
            .Wait();
    }
});
app.Run();

static Action<HostBuilderContext, IConfigurationBuilder> SetupConfiguration()
{
    return (hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;
        config
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
    };
}

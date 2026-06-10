using Microsoft.EntityFrameworkCore;
using Ucms.Common.Extensions;
using Ucms.Stock.Api.Extensions;
using Ucms.Stock.Api.Infrastructure;
using Ucms.Stock.Infrastructure.EntityFramework;

Console.Title = "Stock API";


var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(SetupConfiguration());
builder.Host.UseApplicationSerilog();

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.MigrateDbContext<StockDbContext>((context, services) =>
{
    context.Database.Migrate();

    var config = services.GetRequiredService<IConfiguration>();
    if (config.GetValue<bool>("Database:EnabledDataSeeding"))
    {
        new StockDbContextSeed()
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

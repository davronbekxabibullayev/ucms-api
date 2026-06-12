using Ucms.Api.Extensions;

Console.Title = "UCMS API";

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Host.UseApplicationSerilog();

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.Run();

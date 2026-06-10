namespace Ucms.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;
using Ucms.Application.Persistence;

public class AppDbContextSeed
{
    public async Task SeedAsync(IAppDbContext context, IServiceProvider services)
    {
        var env = services.GetService<IWebHostEnvironment>();
        var logger = services.GetService<ILogger<AppDbContextSeed>>();
        var policy = CreatePolicy(logger!, nameof(AppDbContextSeed));

        await policy.ExecuteAsync(() => Task.CompletedTask);
    }

    private static AsyncRetryPolicy CreatePolicy(ILogger<AppDbContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<NpgsqlException>().WaitAndRetryAsync(retryCount: retries,
            sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
            onRetry: (exception, timeSpan, retry, ctx) =>
            {
                logger.LogWarning(exception,
                    "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                    prefix, exception.GetType().Name, exception.Message, retry, retries);
            });
    }
}

namespace Ucms.Stock.Api.Infrastructure;

using Npgsql;
using Polly;
using Polly.Retry;
using Ucms.Stock.Infrastructure.Persistance;

public class StockDbContextSeed
{
    public async Task SeedAsync(IStockDbContext context, IServiceProvider services)
    {
        var env = services.GetService<IWebHostEnvironment>();
        var logger = services.GetService<ILogger<StockDbContextSeed>>();
        var policy = CreatePolicy(logger!, nameof(StockDbContextSeed));

        await policy.ExecuteAsync(() => Task.CompletedTask);
    }

    private static AsyncRetryPolicy CreatePolicy(ILogger<StockDbContextSeed> logger, string prefix, int retries = 3)
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

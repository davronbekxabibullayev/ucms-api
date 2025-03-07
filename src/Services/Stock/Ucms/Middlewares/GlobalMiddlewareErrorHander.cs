namespace Ucms.Stock.Api.Middlewares;

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ucms.Stock.Domain.Exceptions;

public class GlobalMiddlewareErrorHander
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalMiddlewareErrorHander> _logger;

    public GlobalMiddlewareErrorHander(RequestDelegate next, ILogger<GlobalMiddlewareErrorHander> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);

            if (IsCriticalException(ex))
                _logger.LogError(ex, "Internal server error");

            else
                _logger.LogInformation(ex, "Internal server error");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex.GetStatusCode();
        var problemDetails = ExceptionHandlerExtensions.GetProblemDetails(ex, (int)statusCode);

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode;

        await response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static bool IsCriticalException(Exception ex)
    {
        return ex is not AlreadyExistException and not AccessDeniedException and not NotFoundException;
    }
}

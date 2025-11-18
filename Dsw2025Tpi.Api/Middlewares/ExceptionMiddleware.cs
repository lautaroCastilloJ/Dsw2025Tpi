using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ExceptionBase ex)
        {
            _logger.LogWarning(ex, "Domain exception captured ({Code})", ex.Code);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                code = ex.Code,
                message = ex.Message,
                status = 400,
                traceId = context.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled server error");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                code = "UNEXPECTED_ERROR",
                message = "An unexpected error occurred.",
                status = 500,
                traceId = context.TraceIdentifier
            });
        }
    }
}

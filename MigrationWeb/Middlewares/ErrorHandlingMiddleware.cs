using System.Net;

namespace MigrationWeb.Middlewares;

/// <summary>
/// Centralized error handler middleware.
/// Logs exceptions and returns ProblemDetails for API consumers.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request {Path}", context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";

        var response = new
        {
            status = context.Response.StatusCode,
            title = GetTitle(exception),
            detail = exception.StackTrace,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        InvalidOperationException => "Business Rule Violation",
        ArgumentException => "Invalid Argument",
        KeyNotFoundException => "Resource Not Found",
        _ => "An unexpected error occurred"
    };
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Migration.Contracts.Middlewares;

/// <summary>
/// Centralized error handler for all services.
/// Logs exceptions with CorrelationId and returns ProblemDetails.
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
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}, Path: {Path}",
                correlationId, context.Request.Path);

            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        // Определяем статус-код по типу исключения
        var statusCode = exception switch
        {
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            title = GetTitle(exception),
            detail = exception.StackTrace,
            correlationId = correlationId,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        InvalidOperationException => "Business Rule Violation",
        ArgumentException => "Invalid Argument",
        KeyNotFoundException => "Resource Not Found",
        _ => "An unexpected error occurred"
    };
}
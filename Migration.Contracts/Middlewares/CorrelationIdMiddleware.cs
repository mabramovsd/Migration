using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Migration.Contracts.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get / generate Correlation ID
        string correlationId;
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var headerValue))
        {
            correlationId = headerValue.First()!;
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items["CorrelationId"] = correlationId;

        // Add to log context
        using (_logger.BeginScope(new { CorrelationId = correlationId }))
        {
            context.Response.Headers.Append("X-Correlation-ID", correlationId);
            await _next(context);
        }
    }
}
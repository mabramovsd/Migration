using MigrationWeb.Middlewares;

namespace MigrationWeb;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Overridden error handling
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}

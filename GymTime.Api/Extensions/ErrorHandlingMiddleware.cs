using GymTime.Application.Dtos.Common;
using System.Net;
using System.Text.Json;

namespace GymTime.Api.Extensions;

/// <summary>
/// Middleware para capturar exceções não tratadas e retornar um payload consistente (ErrorResponseDto).
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = MapToStatusCode(exception);
        response.StatusCode = (int)statusCode;

        var error = new ErrorResponseDto { Message = GetErrorMessage(exception, statusCode) };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var payload = JsonSerializer.Serialize(error, options);

        return response.WriteAsync(payload);
    }

    private static HttpStatusCode MapToStatusCode(Exception ex) => ex switch
    {
        KeyNotFoundException => HttpStatusCode.NotFound,
        UnauthorizedAccessException => HttpStatusCode.Unauthorized,
        ArgumentException => HttpStatusCode.BadRequest,
        // add other specific exceptions as needed
        _ => HttpStatusCode.InternalServerError
    };

    private static string GetErrorMessage(Exception ex, HttpStatusCode code)
    {
        // For server errors, avoid leaking internal details.
        if (code == HttpStatusCode.InternalServerError)
            return "An unexpected error occurred.";

        return ex.Message;
    }
}

/// <summary>
/// Extension methods to register the error handling middleware.
/// </summary>
public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Registers the global error handling middleware for IApplicationBuilder-based hosts.
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }

    /// <summary>
    /// Registers the global error handling middleware for minimal hosting (WebApplication).
    /// </summary>
    public static WebApplication UseErrorHandling(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}

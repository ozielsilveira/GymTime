using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace GymTime.Api.Extensions;

/// <summary>
/// Extensions for configuring observability with OpenTelemetry, Prometheus, and Serilog
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Adds OpenTelemetry with Prometheus metrics and tracing
    /// </summary>
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["Observability:ServiceName"] ?? "GymTime.Api";
        var serviceVersion = configuration["Observability:ServiceVersion"] ?? "1.0.0";

        Log.Information("Initializing OpenTelemetry - Service: {ServiceName}, Version: {ServiceVersion}",
            serviceName, serviceVersion);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .WithMetrics(metrics =>
            {
                Log.Information("Configuring OpenTelemetry Metrics with Prometheus exporter");
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                Log.Information("Configuring OpenTelemetry Tracing");
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = (httpContext) =>
                        {
                            // Não trace health checks e métricas
                            var path = httpContext.Request.Path.Value ?? string.Empty;
                            return !path.Contains("/metrics") && !path.Contains("/health");
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });
            });

        Log.Information("OpenTelemetry configured successfully");
        return services;
    }

    /// <summary>
    /// Configures Serilog with Console and File sinks for structured logging
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "GymTime.Api")
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/gymtime-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30)
            .CreateLogger();

        builder.Host.UseSerilog();

        Log.Information("Serilog configured - Environment: {Environment}, Log file: logs/gymtime-{Date}.log",
            environment, DateTime.Now.ToString("yyyyMMdd"));

        return builder;
    }

    /// <summary>
    /// Maps Prometheus metrics endpoint and health check
    /// </summary>
    public static WebApplication UseObservability(this WebApplication app)
    {
        Log.Information("Mapping Prometheus metrics endpoint at /metrics");
        // Prometheus metrics endpoint
        app.MapPrometheusScrapingEndpoint();

        Log.Information("Mapping Health Check endpoint at /health");
        // Health check endpoint
        app.MapGet("/health", () =>
            {
                Log.Debug("Health check endpoint called");
                return Results.Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    service = "GymTime.Api",
                    environment = app.Environment.EnvironmentName
                });
            })
            .WithName("HealthCheck")
            .WithOpenApi()
            .AllowAnonymous();

        Log.Information("Observability endpoints configured successfully");
        return app;
    }
}

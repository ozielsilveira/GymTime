using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;

namespace GymTime.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class RateLimitingExtensions
{
    public static IServiceCollection AddGymRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            // Global policy: Fixed Window per IP (60 req / 1 min)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            options.AddPolicy<string>("LoginPolicy", context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    TokensPerPeriod = 5,
                    AutoReplenishment = true
                });
            });

            // Response when rejected
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                // Indicate retry seconds — here we use 60 seconds as an example
                context.HttpContext.Response.Headers["Retry-After"] = "60";
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", ct);
            };
        });

        return services;
    }

    public static WebApplication UseGymRateLimiting(this WebApplication app)
    {
        // Rate limiter middleware should run early to cut traffic before heavy processing
        app.UseRateLimiter();
        return app;
    }
}
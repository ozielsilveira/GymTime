using System.Text;
using GymTime.Api.Extensions;
using GymTime.Application.Services;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using GymTime.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Configure Serilog logging FIRST
    builder.AddSerilogLogging();

    Log.Information("=== GymTime API Starting ===");
    Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
    Log.Information("Application Name: {ApplicationName}", builder.Environment.ApplicationName);
    Log.Information("Content Root: {ContentRoot}", builder.Environment.ContentRootPath);

    Log.Information("Configuring services...");

    // Add DbContext
    var connectionString = builder.Configuration.GetConnectionString("GymTimeDbConnection");
    Log.Information("Database Connection: {ConnectionString}", connectionString?.Split(';')[0]); // Log only host part

    builder.Services.AddDbContext<GymTimeDbContext>(options =>
     options.UseNpgsql(connectionString ?? throw new Exception("GymTimeDbConnection not found!")));

    // Add repositories
    Log.Information("Registering repositories...");
    builder.Services.AddScoped<IGymMemberRepository, GymMemberRepository>();
    builder.Services.AddScoped<IClassRepository, ClassRepository>();
    builder.Services.AddScoped<IClassSessionRepository, ClassSessionRepository>();
    builder.Services.AddScoped<IBookingRepository, BookingRepository>();

    // Add services
    Log.Information("Registering application services...");
    builder.Services.AddScoped<IBookingService, BookingService>();
    builder.Services.AddScoped<IReportService, ReportService>();
    builder.Services.AddScoped<IGymMemberService, GymMemberService>();
    builder.Services.AddScoped<IClassService, ClassService>();

    // Add Observability (OpenTelemetry + Prometheus)
    Log.Information("Configuring observability (OpenTelemetry + Prometheus)...");
    builder.Services.AddObservability(builder.Configuration);

    // Add JWT Authentication & Authorization
    Log.Information("Configuring JWT Authentication...");
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
   Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "super_secret_key"))
    });

    builder.Services.AddAuthorization();

    // Register rate limiting via extension
    Log.Information("Configuring rate limiting...");
    builder.Services.AddGymRateLimiting(builder.Configuration);

    // Cors policy
    Log.Information("Configuring CORS policy...");
    builder.Services.AddCors(options => options.AddPolicy("DevCorsPolicy", policy =>
       // For development: allow any origin. In production, replace with .WithOrigins("<origin_list>")
       policy.AllowAnyOrigin()
       .AllowAnyHeader()
     .AllowAnyMethod()));

    Log.Information("Configuring controllers...");
    builder.Services.AddControllers(options =>
        // Applies [Authorize] globally; routes that need to be open should use [AllowAnonymous]
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter()));

    // Add Endpoints/Swagger and configure Swagger to accept Bearer token
    Log.Information("Configuring Swagger...");
    builder.Services.AddSwaggerWithJwt();

    Log.Information("Building application...");
    WebApplication app = builder.Build();

    Log.Information("Application built successfully");

    // Apply database migrations automatically
    Log.Information("Checking database migrations...");
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<GymTimeDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                Log.Warning("Found {Count} pending migrations. Applying...", pendingMigrations.Count());
                foreach (var migration in pendingMigrations)
                {
                    Log.Information("  - {Migration}", migration);
                }

                await dbContext.Database.MigrateAsync();
                Log.Information("Database migrations applied successfully!");
            }
            else
            {
                Log.Information("Database is up to date. No pending migrations.");
            }

            // Verify connection
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (canConnect)
            {
                Log.Information("Database connection verified successfully");
            }
            else
            {
                Log.Warning("Database connection could not be verified");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during database migration: {Message}", ex.Message);
            Log.Warning("Application will continue, but database operations may fail");
            // Don't throw - allow app to start even if migrations fail
            // This is useful for health checks and debugging
        }
    }

    Log.Information("Configuring middleware pipeline...");

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
   {
       diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
       diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
       diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
   };
    });

    // Enable Swagger UI only in development (encapsulated)
    app.UseSwaggerInDevelopment();

    // Middleware
    app.UseHttpsRedirection();

    // Global error handling middleware (must be registered early in the pipeline)
    app.UseErrorHandling();

    // Enable rate limiting via extension
    app.UseGymRateLimiting();

    app.UseCors("DevCorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    // Map Observability endpoints (Prometheus metrics + health check)
    Log.Information("Mapping observability endpoints...");
    app.UseObservability();

    app.MapControllers();

    Log.Information("=== GymTime API Configuration Complete ===");
    Log.Information("Listening on: {Urls}", string.Join(", ", builder.Configuration.GetSection("Kestrel:Endpoints")
 .GetChildren()
        .Select(x => x.Key)
      .DefaultIfEmpty("default URLs")));

    Log.Information("Available endpoints:");
    Log.Information("  - Health Check: /health");
    Log.Information("  - Metrics (Prometheus): /metrics");
    Log.Information("  - Swagger UI: / (Development only)");
    Log.Information("  - API: /api/*");

    Log.Information("=== Starting Kestrel Web Server ===");
    app.Run();

    Log.Information("=== GymTime API Stopped Gracefully ===");
}
catch (Exception ex)
{
    Log.Fatal(ex, "=== APPLICATION FAILED TO START ===");
    Log.Fatal("Error Message: {Message}", ex.Message);
    Log.Fatal("Stack Trace: {StackTrace}", ex.StackTrace);
    throw;
}
finally
{
    Log.Information("=== Closing and flushing logs ===");
    Log.CloseAndFlush();
}

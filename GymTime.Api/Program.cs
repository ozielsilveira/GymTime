using GymTime.Api.Extensions;
using GymTime.Application.Services;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using GymTime.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<GymTimeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GymTimeDbConnection") ?? throw new Exception("GymTimeDbConnection not found!")));

// Add repositories
builder.Services.AddScoped<IGymMemberRepository, GymMemberRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Add services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IGymMemberService, GymMemberService>();
builder.Services.AddScoped<IClassService, ClassService>();

// Add JWT Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "super_secret_key"))
    };
});

builder.Services.AddAuthorization();

// Register rate limiting via extension
builder.Services.AddGymRateLimiting(builder.Configuration);

// Cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        // For development: allow any origin. In production, replace with .WithOrigins("<origin_list>")
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options =>
{
    // Applies [Authorize] globally; routes that need to be open should use [AllowAnonymous]
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

// Add Endpoints/Swagger and configure Swagger to accept Bearer token
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Enable Swagger UI only in development (encapsulated)
app.UseSwaggerInDevelopment();

// Middleware
app.UseHttpsRedirection();

// Enable rate limiting via extension
app.UseGymRateLimiting();

app.UseCors("DevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();